using UnityEditor;
using UnityEngine;

public class WebGLTextureSettings {

    private static int _size = 512;

    [MenuItem("Tools/WebGL: Alle Texturen auf XXX + Crunch setzen")]
    static void ApplyWebGLSettings() {

        string[] guids = AssetDatabase.FindAssets("t:Texture2D");
        int count = 0;

        foreach (string guid in guids) {

            string path = AssetDatabase.GUIDToAssetPath(guid);

            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;

            if (ti != null) {
                bool changed = false;

                if (ti.maxTextureSize != _size) {
                    ti.maxTextureSize = _size;
                    changed = true;
                }

                if (ti.textureCompression != TextureImporterCompression.CompressedHQ) {
                    ti.textureCompression = TextureImporterCompression.CompressedHQ;
                    changed = true;
                }

                if (!ti.crunchedCompression) {
                    ti.crunchedCompression = true;
                    ti.compressionQuality = 50;
                    changed = true;
                }

                // Optional: Plattform-Override für WebGL explizit setzen
                ti.SetPlatformTextureSettings(new TextureImporterPlatformSettings {
                    maxTextureSize = _size,
                    format = TextureImporterFormat.ASTC_6x6, // oder ASTC_4x4
                    overridden = true,
                    name = "WebGL"
                });

                if (changed) {
                    ti.SaveAndReimport();
                    count++;
                }
            }
        }

        Debug.Log($"Fertig: {count} Texturen für WebGL optimiert (1024 + Crunch + ASTC)");
    }
}