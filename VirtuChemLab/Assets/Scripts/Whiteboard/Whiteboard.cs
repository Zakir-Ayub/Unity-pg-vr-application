using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;

public class Whiteboard : NetworkBehaviour
{
    public Texture2D texture;
    public Vector2 textureSize = new Vector2(x: 2048, y: 2048);

   
    void Start()
    {
        var r = GetComponent <Renderer>();
        
        texture = new Texture2D(width: (int)textureSize.x, height: (int)textureSize.y);
        
        for (int w=0; w<texture.height; ++w) 
	    {
		    for (int q= 0; q< texture.width; ++q) 
		    {
			    texture.SetPixel (q, w, Color.white);
		    }
	    } 
	    // Apply all SetPixel calls
	    texture.Apply();
        r.material.mainTexture = texture;

        //Initial syncing of the texture doesnt work, cause its  too large. Need to fix that.
        //NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
    }

    public void OnClientConnectedCallback(ulong clientId)
    {
        if (IsServer) {
            Debug.Log("Client detected, Sending Whiteboard");
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            };
            byte[] textureData = texture.EncodeToPNG();
            initTextureClientRpc(texture, clientRpcParams);
        }
    }

    //
    [ClientRpc]
    public void initTextureClientRpc(Texture2D texture, ClientRpcParams clientRpcParams)
    {
        Debug.Log("Client received whiteboard  texture from server");
        this.texture = texture;
    }

    public void setPixelsToTexture(int x, int y, int penSizeX, int penSizeY, Color color)
    {
        if(IsServer)
        {
            setPixelsToTextureClientRPC(x, y, penSizeX, penSizeY, color);
        }
        Color[] _colors = Enumerable.Repeat(color, penSizeX * penSizeY).ToArray();
        texture.SetPixels(x, y, penSizeX, penSizeY, _colors);
    }
    
    [ClientRpc]
    public void setPixelsToTextureClientRPC(int x, int y, int penSizeX, int penSizeY, Color color)
    {
        Color[] _colors = Enumerable.Repeat(color, penSizeX * penSizeY).ToArray();
        texture.SetPixels(x, y, penSizeX, penSizeY, _colors);
    }

    public void applyToTexture()
    {
        if(IsServer)
        {
            applyToTextureClientRPC();
        }
        texture.Apply();
    }

    [ClientRpc]
    public void applyToTextureClientRPC()
    {
        texture.Apply();
    }
    
}
