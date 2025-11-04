#!/bin/bash
set -euo pipefail

# === CONFIG ===
PROJECT_PATH="/Users/roshil/WbidMax-MACDotnet/WBidMac/WBidMac/WBidMac.csproj"
APP_NAME="WBidMac"
BUILD_DIR="/Users/roshil/WbidMax-MACDotnet/WBidMac/WBidMac/bin/Release/net8.0-macos"
APP_PATH="$BUILD_DIR/universal/${APP_NAME}.app"
ZIP_PATH="/Users/roshil/${APP_NAME}.zip"
IDENTITY="Developer ID Application: WBID, LLC (44J23FC632)"
PROFILE="notary-credentials"
ENTITLEMENTS="/Users/roshil/WbidMax-MACDotnet/WBidMac/WBidMac/entitlements.plist"

# === CLEAN ===
rm -rf "$BUILD_DIR"
mkdir -p "$BUILD_DIR"

# === BUILD ===
echo "👉 Building arm64..."
dotnet publish "$PROJECT_PATH" -c Release -r osx-arm64 --self-contained true

echo "👉 Building x64..."
dotnet publish "$PROJECT_PATH" -c Release -r osx-x64 --self-contained true

APP_ARM64="$BUILD_DIR/osx-arm64/${APP_NAME}.app"
APP_X64="$BUILD_DIR/osx-x64/${APP_NAME}.app"

# === CREATE UNIVERSAL APP ===
echo "👉 Creating universal app..."
mkdir -p "$(dirname "$APP_PATH")"
cp -R "$APP_ARM64" "$APP_PATH"

# Merge main executable
lipo -create \
  "$APP_ARM64/Contents/MacOS/${APP_NAME}" \
  "$APP_X64/Contents/MacOS/${APP_NAME}" \
  -output "$APP_PATH/Contents/MacOS/${APP_NAME}"

echo "✅ Universal binary created at: $APP_PATH"

# === MERGE MONOBUNDLE LIBRARIES ===
echo "👉 Merging runtime libraries (MonoBundle)..."
for ARM_FILE in "$APP_ARM64/Contents/MonoBundle/"*.dylib; do
  REL_NAME=$(basename "$ARM_FILE")
  X64_FILE="$APP_X64/Contents/MonoBundle/$REL_NAME"
  UNIVERSAL_FILE="$APP_PATH/Contents/MonoBundle/$REL_NAME"

  if [[ -f "$X64_FILE" ]]; then
    echo "   Merging $REL_NAME"
    lipo -create "$ARM_FILE" "$X64_FILE" -output "$UNIVERSAL_FILE"
  else
    echo "   ⚠️ No x64 match for $REL_NAME (keeping arm64 only)"
    cp "$ARM_FILE" "$UNIVERSAL_FILE"
  fi
done

# === SIGN LIBRARIES ===
echo "👉 Signing libraries (.dylib, .so)..."
find "$APP_PATH" -type f \( -name "*.dylib" -o -name "*.so" \) -exec \
  codesign --force --options runtime --timestamp --sign "$IDENTITY" {} \;

# === SIGN MAIN EXECUTABLE ===
echo "👉 Signing main executable..."
codesign --force --options runtime --timestamp \
  --entitlements "$ENTITLEMENTS" \
  --sign "$IDENTITY" "$APP_PATH/Contents/MacOS/${APP_NAME}"

# === SIGN APP BUNDLE ===
echo "👉 Signing app bundle..."
codesign --deep --force --options runtime --timestamp \
  --entitlements "$ENTITLEMENTS" \
  --sign "$IDENTITY" "$APP_PATH"

# === VERIFY ===
echo "👉 Verifying signature..."
codesign -dv --verbose=4 "$APP_PATH" || true
spctl --assess --verbose "$APP_PATH" || true

# === ZIP FOR NOTARIZATION ===
echo "👉 Creating ZIP..."
cd "$(dirname "$APP_PATH")"
zip -r "$ZIP_PATH" "$(basename "$APP_PATH")"

# === NOTARIZE ===
echo "👉 Submitting to Apple Notary Service..."
xcrun notarytool submit "$ZIP_PATH" --keychain-profile "$PROFILE" --wait

# === STAPLE ===
echo "👉 Stapling ticket..."
xcrun stapler staple "$APP_PATH"

echo "🎉 Done! Universal app built, signed, notarized, and stapled!"
