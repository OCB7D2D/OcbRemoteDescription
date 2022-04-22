# OCB Remote Description Mod - 7 Days to Die (A20) Addon

This mod is meant to be used by other mods in order to
provide remote information when looking at blocks. On a
dedicated server, a lot of information is not readily
available on the client side. This limits what can be
shown when looking at blocks.

This mod solves this by pinging the server when we look
at a block. Actually only blocks that have been configured
to use remote descriptions, in order to preserve bandwidth.
The server will then answer with the requested custom label.

We do this repeatedly when staring at one block in order to
get up to date information from the server. In case the
server doesn't answer, the last provided label is used,
until it either goes stale or an update is received.

This gives a fair balance between bandwidth usage and added
value, as we only ask for an update about once per second.

See [OcbPlantGrowInfo][1] for a demo implementation.

[![GitHub CI Compile Status][3]][2]

## Compatibility

I've developed and tested this Mod against version a20.3(b3).

[1]: https://github.com/OCB7D2D/OcbPlantGrowInfo
[2]: https://github.com/OCB7D2D/OcbRemoteDescription/actions/workflows/ci.yml
[3]: https://github.com/OCB7D2D/OcbRemoteDescription/actions/workflows/ci.yml/badge.svg
