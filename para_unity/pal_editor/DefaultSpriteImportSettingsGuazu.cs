using UnityEngine;
using UnityEditor;
using System.Collections;

public class DefaultSpriteImportSettingsGuazu : AssetPostprocessor {
    void OnPreprocessTexture()
    {
        TextureImporter textureImporter = (TextureImporter)assetImporter;
        if (textureImporter.userData.Length > 0) return;
        TextureImporterSettings tis = new TextureImporterSettings();
        textureImporter.ReadTextureSettings(tis);
        tis.ApplyTextureType(TextureImporterType.Sprite, false);
        if (textureImporter.DoesSourceTextureHaveAlpha())tis.alphaIsTransparency = true;
        tis.filterMode = FilterMode.Bilinear;
        tis.mipmapEnabled = false;
        tis.spriteBorder = Vector4.zero;
        tis.spritePixelsPerUnit = 1;
        tis.textureFormat = TextureImporterFormat.AutomaticTruecolor;
        tis.wrapMode = TextureWrapMode.Clamp;
        tis.npotScale = TextureImporterNPOTScale.None;
        tis.spriteAlignment = (int)SpriteAlignment.Custom;
        tis.spriteExtrude = 0;
        tis.spriteMeshType = SpriteMeshType.Tight;
        textureImporter.userData = "X";
        textureImporter.SetTextureSettings(tis);
    }
}
