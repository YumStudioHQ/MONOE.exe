using System.Collections.Generic;
using Godot;
using monoe.exe.Core.Manager;

namespace monoe.exe.Core.Bridge.Types;

public class MTileMap : Exposable
{
  private readonly TileMapLayer map = new()
  {
    TileSet = new(),
    GlobalPosition = Vector2.Zero
  };

  public void PlaceTile(long x, long y, long tileIndex, long tX, long tY)
  {
    map.SetCell(new((int)x, (int)y), (int)tileIndex, new((int)tX, (int)tY));
  }

  private long _AddImage(Image image, Vector2I tileSize)
  {
    map.TileSet.TileSize = tileSize;
    var source = new TileSetAtlasSource
    {
      Texture = image.Texture,
      TextureRegionSize = tileSize
    };

    int sourceId = map.TileSet.AddSource(source);

    // Compute how many tiles fit in the texture
    Vector2I texSize = (Vector2I)source.Texture.GetSize();
    int tilesX = texSize.X / tileSize.X;
    int tilesY = texSize.Y / tileSize.Y;

    for (int y = 0; y < tilesY; y++)
    {
      for (int x = 0; x < tilesX; x++)
      {
        source.CreateTile(new Vector2I(x, y));
      }
    }

    return sourceId;
  }

  public long AddImage(long uid, long tileW, long tileH)
  {
    if (ObjectRegistry.TryGet(uid, out Image value))
    {
      return _AddImage(value, new Vector2I((int)tileW, (int)tileH));
    }
    else
    {
      ObjectRegistry.TryGet(uid, out object o);
      throw new KeyNotFoundException($"{uid}: Not an image! Got: {o?.GetType()?.FullName}");
    }
  }

  public void Scale(double x, double y)
  {
    map.Scale = new((float)x, (float)y);
  }

  public object[] ToLocal(double x, double y)
  {
    Vector2I vec = map.LocalToMap(new Vector2((float)x, (float)y));
    return [vec.X, vec.Y];
  }

  public object[] ToGlobal(long x, long y)
  {
    Vector2 vec = map.MapToLocal(new Vector2I((int)x, (int)y));
    vec = map.ToGlobal(vec);
    return [vec.X, vec.Y];
  }

  public override Node NRef()
   => map;

  public override void Remove()
   => map.GetParent().RemoveChild(map);

  protected override void _Free()
   => map.QueueFree();
}