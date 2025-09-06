#!/bin/sh
set -e
# Need root to fix perms on a fresh volume
if [ "$(id -u)" -ne 0 ]; then
  exec /bin/sh -lc "exec \"$0\" \"$@\""
fi

mkdir -p /Downloads
chown -R app:app /Downloads
chmod -R 755 /Downloads

exec gosu app:app dotnet Mp3Tagger.Web.dll
