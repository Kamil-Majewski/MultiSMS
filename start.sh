#!/usr/bin/env bash

URL="0.0.0.0:5001"

cd -- "$(dirname -- "$0")" || exit 1

cd MultiSMS.MVC
dotnet run . --urls="https://$URL"