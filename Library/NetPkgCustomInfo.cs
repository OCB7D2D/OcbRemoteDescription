public class NetPkgCustomInfo : NetPackage
{

    public static Vector3i LastPosition = Vector3i.zero;
    public static int LastType = BlockValue.Air.type;
    public static string LastText = string.Empty;
    public static ulong LastTick = 0;

    private Vector3i Position = Vector3i.zero;
    private int BlockType = BlockValue.Air.type;
    private string Text = string.Empty;

    // Request server to answer with custom description
    // We currently abuse the same package for both ways
    // Could reduce the overhead a little by using either
    // A dedicated class or some dynamic switch flag 
    public NetPkgCustomInfo ToServer(Vector3i position)
    {
        Position = position == null ? Vector3i.zero : position;
        Text = string.Empty;
        BlockType = 0;
        return this;
    }

    // Provide custom description to the client
    public NetPkgCustomInfo ToClient(Vector3i position, int type, string text)
    {
        Position = position == null ? Vector3i.zero : position;
        Text = text == null ? string.Empty : text;
        BlockType = type;
        return this;
    }

    public override void read(PooledBinaryReader _br)
    {
        Position = new Vector3i(
            _br.ReadInt32(),
            _br.ReadInt32(),
            _br.ReadInt32());
        Text = _br.ReadString();
        BlockType = _br.ReadInt32();
    }

    public override void write(PooledBinaryWriter _bw)
    {
        base.write(_bw);
        _bw.Write(Position.x);
        _bw.Write(Position.y);
        _bw.Write(Position.z);
        _bw.Write(Text);
        _bw.Write(BlockType);
    }

    public override void ProcessPackage(World _world, GameManager _callbacks)
    {
        if (_world == null) return;
        if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
        {
            LastTick = GameTimer.Instance.ticks;
            LastPosition = Position;
            LastText = Text;
            LastType = BlockType;
        }
        else
        {
            BlockValue block = _world.GetBlock(Position);
            string info = "SERVER AIRROR";
            if (block.type != BlockValue.Air.type)
            {
                // Invoke the original implementation to get the string
                info = block.Block.GetCustomDescription(Position, block);
            }
            // Send information to connected client
            Sender.SendPackage(NetPackageManager
                .GetPackage<NetPkgCustomInfo>()
                    .ToClient(Position, block.type, info));
        }
    }

    public override int GetLength() => 28 + Text.Length;
}
