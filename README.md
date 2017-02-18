# Aperture

Aperture is a 32-bit application that redirects the standard input and output
of console applications to a connected socket handle in the spirit of the old
PC Micro "DoorWay" program. This can be used to turn an ordinary console
application into a BBS door.

## Usage

    aperture.exe -H<handle> -N<node> -- <command> <arguments>

Node is unnecessary. You can use `-N1` as a default.
