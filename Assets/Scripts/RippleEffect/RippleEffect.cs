using UnityEngine;
using System.Collections;

public class RippleEffect : MonoBehaviour
{
    public readonly AnimationCurve Waveform = new(
        new Keyframe(0.00f, 0.50f, 0, 0),
        new Keyframe(0.05f, 1.00f, 0, 0),
        new Keyframe(0.15f, 0.10f, 0, 0),
        new Keyframe(0.25f, 0.80f, 0, 0),
        new Keyframe(0.35f, 0.30f, 0, 0),
        new Keyframe(0.45f, 0.60f, 0, 0),
        new Keyframe(0.55f, 0.40f, 0, 0),
        new Keyframe(0.65f, 0.55f, 0, 0),
        new Keyframe(0.75f, 0.46f, 0, 0),
        new Keyframe(0.85f, 0.52f, 0, 0),
        new Keyframe(0.99f, 0.50f, 0, 0)
    );

    [Range(0.01f, 1.0f)]
    public float RefractionStrength = 0.5f;

    public Color ReflectionColor = Color.gray;

    [Range(0.01f, 1.0f)]
    public float ReflectionStrength = 0.7f;

    [Range(1.0f, 5.0f)]
    public float WaveSpeed = 1.25f;

    [Range(0.0f, 2.0f)]
    public float DropInterval = 0.5f;

    [SerializeField, HideInInspector]
    Shader Shader;

    class Droplet
    {
        Vector2 Position;
        float Time = 1000;

        public void Reset(Vector2 pos)
        {
			Position = pos;
            Time = 0;
        }
        public void Update() => Time += UnityEngine.Time.deltaTime * 2;
        public Vector4 MakeShaderParameter(float aspect) => new(Position.x * aspect, Position.y, Time, 0);
    }

    readonly Droplet[] Droplets = { new Droplet(), new Droplet(), new Droplet() };
    readonly Texture2D GradTexture = new(2048, 1, TextureFormat.Alpha8, false) 
    { 
        wrapMode = TextureWrapMode.Clamp,
        filterMode = FilterMode.Bilinear
    };
    Material Material;
    float Timer;
    int DropCount;

    void UpdateShaderParameters()
    {
        Camera c = GetComponent<Camera>();

        Material.SetVector("_Drop1", Droplets[0].MakeShaderParameter(c.aspect));
        Material.SetVector("_Drop2", Droplets[1].MakeShaderParameter(c.aspect));
        Material.SetVector("_Drop3", Droplets[2].MakeShaderParameter(c.aspect));

        Material.SetColor("_Reflection", ReflectionColor);
        Material.SetVector("_Params1", new Vector4(c.aspect, 1, 1 / WaveSpeed, 0));
        Material.SetVector("_Params2", new Vector4(1, 1 / c.aspect, RefractionStrength, ReflectionStrength));
    }

    void Awake()
    {
        for (int i = 0; i < GradTexture.width; i++)
        {
            float x = 1.0f / GradTexture.width * i;
            float a = Waveform.Evaluate(x);
            GradTexture.SetPixel(i, 0, new Color(a, a, a, a));
        }
        GradTexture.Apply();

        Material = new Material(Shader)
        {
            hideFlags = HideFlags.DontSave
        };
        Material.SetTexture("_GradTex", GradTexture);

        UpdateShaderParameters();
    }

    void Update()
    {
        if (DropInterval > 0)
        {
            Timer += Time.deltaTime;
            while (Timer > DropInterval) Timer -= DropInterval;
        }

        foreach (Droplet d in Droplets) d.Update();

        UpdateShaderParameters();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, Material);
    }

    public void Emit(Vector2 pos)
    {
        Droplets[DropCount++ % Droplets.Length].Reset(pos);
    }

    IEnumerator Stop()
    {
        yield return new WaitForSeconds(.3f);
    }

}
