using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShaderDriver : MonoBehaviour
{

    [SerializeField] private Camera camera;
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private float a;
    [SerializeField] private float b;
    [SerializeField] private float c;
    [SerializeField] private float d;

    private ComputeBuffer positionsBuffer;
    private Vector2[] positions;
    [SerializeField] private Vector2Int dimensions;
    private int numParticles = 51200;

    private int mainId;
    private int renderId;
    private int clearTextureId;
    private int fadeTextureId;
    
    // Start is called before the first frame update
    private void Awake()
    {
        //dimensions = new Vector2Int(Screen.width, Screen.height);

        Application.targetFrameRate = 60;
        mainId = computeShader.FindKernel("Main");
        renderId = computeShader.FindKernel("Render");
        clearTextureId = computeShader.FindKernel("ClearTexture");
        fadeTextureId = computeShader.FindKernel("FadeTexture");
        
        renderTexture = new RenderTexture(dimensions.x, dimensions.y, 16);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();
        computeShader.SetTexture(renderId, "renderTexture", renderTexture);
        computeShader.SetTexture(clearTextureId, "renderTexture", renderTexture);
        computeShader.SetTexture(fadeTextureId, "renderTexture", renderTexture);
        
        positionsBuffer = new ComputeBuffer(numParticles, sizeof(float) * 2);
        
        ResetSim();

        computeShader.SetVector("offset", new Vector2(renderTexture.width / 2f, renderTexture.height / 2f));
    }

    private void Update()
    {
        computeShader.SetVector("p", new Vector4(a, b, c, d));
        computeShader.Dispatch(mainId, positions.Length / 128, 1, 1);

        if (Input.GetKeyDown(KeyCode.R)) ResetSim();
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            RenderTexture.active = null;
            
            var bytes = texture.EncodeToPNG();

            var path = "screenshot.png";
            File.WriteAllBytes(path, bytes);
            Debug.Log("took screenshot");
        }
    }

    private void ResetSim()
    {
        positions = new Vector2[numParticles];
        
        for (var i = 0; i < positions.Length; i++)
        {
            var randX = Random.Range(-8f, 8f);
            var randY = Random.Range(-8f, 8f);
            positions[i] = new Vector2(randX, randY);
        }
        
        positionsBuffer.SetData(positions);
        computeShader.SetBuffer(mainId, "positions", positionsBuffer);
        computeShader.SetBuffer(renderId, "positions", positionsBuffer);
        
        computeShader.Dispatch(clearTextureId, renderTexture.width / 32 + 1, renderTexture.height / 8 + 1, 1);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //computeShader.Dispatch(clearTextureId, renderTexture.width / 32 + 1, renderTexture.height / 8 + 1, 1);
        computeShader.Dispatch(renderId, positions.Length / 128, 1, 1);
        //computeShader.Dispatch(fadeTextureId, renderTexture.width / 32 + 1, renderTexture.height / 8 + 1, 1);
        Graphics.Blit(renderTexture, destination);
    }
}
