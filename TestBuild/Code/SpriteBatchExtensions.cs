using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public static class SpriteBatchExtensions
{
    private static Texture2D _whiteTexture;

    private static void EnsureWhiteTextureExists(SpriteBatch spriteBatch)
    {
        if (_whiteTexture == null)
        {
            _whiteTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            _whiteTexture.SetData(new[] { Color.White });
        }
    }

    public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rectangle, Color color)
    {
        EnsureWhiteTextureExists(spriteBatch);

        // Draw top line
        spriteBatch.Draw(_whiteTexture, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, 1), color);

        // Draw left line
        spriteBatch.Draw(_whiteTexture, new Rectangle(rectangle.Left, rectangle.Top, 1, rectangle.Height), color);

        // Draw right line
        spriteBatch.Draw(_whiteTexture, new Rectangle(rectangle.Right - 1, rectangle.Top, 1, rectangle.Height), color);

        // Draw bottom line
        spriteBatch.Draw(_whiteTexture, new Rectangle(rectangle.Left, rectangle.Bottom - 1, rectangle.Width, 1), color);
    }
}