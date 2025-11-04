#!/bin/bash
set -euo pipefail

APP_PATH="/Users/roshil/WbidMax-MACDotnet/WBidMac/WBidMac/bin/Release/net8.0-macos/universal/WBidMac.app"
ARM_PATH="/Users/roshil/WbidMax-MACDotnet/WBidMac/WBidMac/bin/Release/net8.0-macos/osx-arm64/WBidMac.app"
X64_PATH="/Users/roshil/WbidMax-MACDotnet/WBidMac/WBidMac/bin/Release/net8.0-macos/osx-x64/WBidMac.app"

echo "🔍 Checking for non-universal binaries in:"
echo "   $APP_PATH"
echo ""

find "$APP_PATH" -type f \( -name "*.dylib" -o -name "*.so" -o -name "WBidMac" \) | while read -r FILE; do
    INFO=$(lipo -info "$FILE" 2>&1 || true)

    if [[ "$INFO" == *"Non-fat file"* ]]; then
        echo "❌ $FILE → $INFO"
        # check if x64 version exists
        REL_PATH="${FILE#$APP_PATH/}"
        ARM_FILE="$ARM_PATH/$REL_PATH"
        X64_FILE="$X64_PATH/$REL_PATH"
        if [[ -f "$X64_FILE" ]]; then
            echo "   ✅ Found matching x64 file at: $X64_FILE"
        else
            echo "   ⚠️ No x64 version found → Intel crash risk"
        fi
    elif [[ "$INFO" == *"x86_64 arm64"* ]]; then
        echo "✅ $FILE → Universal"
    else
        echo "⚠️ $FILE → $INFO"
    fi
done
