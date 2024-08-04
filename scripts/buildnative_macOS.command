#!/bin/bash
dotnet publish ../src/ -r osx-x64 /p:Configuration=Release /p:platform="x64"
