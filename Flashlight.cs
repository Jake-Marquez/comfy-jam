using Godot;
using System.Collections.Generic;

// Darkens every CanvasItem in DarkGroup except inside a circle around the
// flashlight point (the mouse, by default).
//
// A node can override how dark it gets by setting a "darkness" metadata
// value (0 = untouched, 1 = fully tinted). Others use DefaultDarkness.
public partial class Flashlight : Node
{
	[Export] public string DarkGroup = "dark";
	[Export] public float Radius = 120f;
	[Export] public float Softness = 40f;
	[Export(PropertyHint.Range, "0,1")] public float DefaultDarkness = 0.85f;
	[Export] public bool On = true;
	// When false, set LightPosition yourself (e.g. from a controller stick).
	[Export] public bool FollowMouse = true;

	// Flashlight point in viewport pixels.
	public Vector2 LightPosition;

	private Shader _shader;
	// One material per darkness level, shared by every node using that level.
	private readonly Dictionary<float, ShaderMaterial> _materials = new();

	public override void _Ready()
	{
		_shader = GD.Load<Shader>("res://shaders/darkness.gdshader");

		foreach (Node node in GetTree().GetNodesInGroup(DarkGroup))
		{
			if (node is CanvasItem item)
				Darken(item);
		}

		// Anything added to the group later (e.g. spawned thieves) gets darkened too.
		GetTree().NodeAdded += node =>
		{
			if (node is CanvasItem item && item.IsInGroup(DarkGroup))
				Darken(item);
		};
	}

	public override void _Process(double delta)
	{
		Viewport viewport = GetViewport();
		if (FollowMouse)
			LightPosition = viewport.GetMousePosition();

		Vector2 viewportSize = viewport.GetVisibleRect().Size;
		foreach (ShaderMaterial material in _materials.Values)
		{
			material.SetShaderParameter("light_pos", LightPosition);
			material.SetShaderParameter("viewport_size", viewportSize);
			material.SetShaderParameter("radius", Radius);
			material.SetShaderParameter("softness", Softness);
			material.SetShaderParameter("light_on", On);
		}
	}

	public void Darken(CanvasItem item)
	{
		float darkness = item.HasMeta("darkness") ? item.GetMeta("darkness").AsSingle() : DefaultDarkness;
		item.Material = GetMaterial(darkness);
		// Children without their own material inherit the darkness.
		foreach (Node child in item.FindChildren("*", "CanvasItem", true, false))
		{
			if (child is CanvasItem c && c.Material == null)
				c.UseParentMaterial = true;
		}
	}

	// True if the item's origin is inside the beam, e.g. to scare off a thief.
	public bool IsLit(CanvasItem item)
	{
		Vector2 screenPos = item.GetGlobalTransformWithCanvas().Origin;
		return On && screenPos.DistanceTo(LightPosition) <= Radius;
	}

	private ShaderMaterial GetMaterial(float darkness)
	{
		if (!_materials.TryGetValue(darkness, out ShaderMaterial material))
		{
			material = new ShaderMaterial { Shader = _shader };
			material.SetShaderParameter("darkness", darkness);
			_materials[darkness] = material;
		}
		return material;
	}
}
