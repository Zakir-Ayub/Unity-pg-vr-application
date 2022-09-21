using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public static class TextureSerializer
{
    public static void ReadValueSafe(this FastBufferReader reader, out Texture2D texture)
    {
        reader.ReadValueSafe(out int width);
        reader.ReadValueSafe(out int height);
        reader.ReadValueSafe(out byte[] byteArray);
        texture = new Texture2D(width, height, TextureFormat.RFloat, false);
        texture.Apply(false);
        texture.LoadImage(byteArray);
    } //ReadValueSafe

    public static void WriteValueSafe(this FastBufferWriter writer, in Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;
        byte[] byteArray = texture.EncodeToPNG();
        writer.WriteValueSafe(width);
        writer.WriteValueSafe(height);
        writer.WriteValueSafe(byteArray);
    } //WriteValueSafe
} //SerializationExtensions