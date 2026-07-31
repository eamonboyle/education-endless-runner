# Android Signing

The previous in-repo `user.keystore` has been removed. Do **not** commit signing keys.

## Generate a release keystore (once)

```bash
keytool -genkeypair -v \
  -keystore math-runner-release.keystore \
  -alias math-runner-release \
  -keyalg RSA -keysize 2048 -validity 10000 \
  -dname "CN=Math Runner, OU=Mobile, O=Eamon Boyle, C=IE"
```

Store the keystore file and passwords in a password manager. Keep an offline backup.

## Local Unity builds

1. Place `math-runner-release.keystore` somewhere outside the repo (e.g. `~/keys/`).
2. In Unity: **Edit → Project Settings → Player → Android → Publishing Settings**
3. Enable Custom Keystore and point at that file / alias.

`ProjectSettings` currently has `androidUseCustomKeystore: 0` so debug/dev builds use the Unity debug key. Flip it on only when building a store AAB.

## CI secrets (GameCI)

Store these GitHub Actions secrets:

| Secret | Value |
|--------|--------|
| `ANDROID_KEYSTORE_BASE64` | `base64 -w0 math-runner-release.keystore` |
| `ANDROID_KEYSTORE_PASS` | keystore password |
| `ANDROID_KEYALIAS_NAME` | `math-runner-release` |
| `ANDROID_KEYALIAS_PASS` | key password |

Decode in the workflow before `unity-builder`:

```yaml
- name: Decode keystore
  run: echo "${{ secrets.ANDROID_KEYSTORE_BASE64 }}" | base64 -d > math-runner-release.keystore
```

Then pass the path/passwords to `game-ci/unity-builder` via its Android signing inputs.

## Play App Signing

On first Play Console upload, enrol in **Play App Signing** so Google holds the app signing key. That makes future upload-key rotation recoverable if the local keystore is lost.

## Purging the old key from git history

`user.keystore` was previously committed. Removing it from the working tree is not enough for a leaked key:

```bash
git filter-repo --path user.keystore --invert-paths
# or: BFG --delete-files user.keystore
```

Then force-push and treat the old key as compromised (it cannot be used for the new package id `com.eamonboyle.LearningEndlessRunner` anyway).
