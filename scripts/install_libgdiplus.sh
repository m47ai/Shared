#!/bin/bash

apt update
apt install -y libgdiplus
ln -s /usr/lib/x86_64-linux-gnu/libdl.so.2 /usr/lib/x86_64-linux-gnu/libdl.so
ln -s /usr/lib/x86_64-linux-gnu/libgdiplus.so /usr/lib/x86_64-linux-gnu/libgdiplus.so