using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoGameTest
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Model _sphereModel;
        private Texture2D _rainbowTexture;
        private BasicEffect _effect;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _sphereModel = CreateSphereModel();
            _rainbowTexture = CreateRainbowTexture();

            _effect = new BasicEffect(GraphicsDevice)
            {
                TextureEnabled = true,
                Texture = _rainbowTexture,
                LightingEnabled = false,
                VertexColorEnabled = false
            };
        }

        private Model CreateSphereModel()
        {
            // Create a sphere model programmatically
            int tessellation = 16;
            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[(tessellation + 1) * (tessellation + 1)];
            int[] indices = new int[tessellation * tessellation * 6];

            int vertexCount = 0;
            int indexCount = 0;

            for (int i = 0; i <= tessellation; i++)
            {
                float latitude = (float)i * MathHelper.Pi / tessellation;
                float dy = (float)Math.Cos(latitude);
                float dxz = (float)Math.Sin(latitude);

                for (int j = 0; j <= tessellation; j++)
                {
                    float longitude = (float)j * MathHelper.TwoPi / tessellation;
                    float dx = (float)Math.Sin(longitude);
                    float dz = (float)Math.Cos(longitude);

                    Vector3 normal = new Vector3(dx * dxz, dy, dz * dxz);
                    Vector2 textureCoordinate = new Vector2((float)j / tessellation, (float)i / tessellation);

                    vertices[vertexCount++] = new VertexPositionNormalTexture(normal, normal, textureCoordinate);

                    if (i < tessellation && j < tessellation)
                    {
                        indices[indexCount++] = i * (tessellation + 1) + j;
                        indices[indexCount++] = (i + 1) * (tessellation + 1) + j;
                        indices[indexCount++] = i * (tessellation + 1) + j + 1;

                        indices[indexCount++] = i * (tessellation + 1) + j + 1;
                        indices[indexCount++] = (i + 1) * (tessellation + 1) + j;
                        indices[indexCount++] = (i + 1) * (tessellation + 1) + j + 1;
                    }
                }
            }

            VertexBuffer vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionNormalTexture), vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);

            IndexBuffer indexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);

            ModelMeshPart meshPart = new ModelMeshPart
            {
                VertexBuffer = vertexBuffer,
                IndexBuffer = indexBuffer,
                NumVertices = vertices.Length,
                PrimitiveCount = indices.Length / 3,
                StartIndex = 0,
                VertexOffset = 0
            };

            ModelMesh mesh = new ModelMesh(GraphicsDevice, new[] { meshPart });
            Model model = new Model(GraphicsDevice, new[] { mesh });

            return model;
        }

        private Texture2D CreateRainbowTexture()
        {
            // Create a rainbow texture programmatically
            int width = 256;
            int height = 256;
            Texture2D texture = new Texture2D(GraphicsDevice, width, height);

            Color[] colors = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float hue = (float)x / width;
                    colors[y * width + x] = ColorFromHue(hue);
                }
            }

            texture.SetData(colors);
            return texture;
        }

        private Color ColorFromHue(float hue)
        {
            float r = Math.Abs(hue * 6 - 3) - 1;
            float g = 2 - Math.Abs(hue * 6 - 2);
            float b = 2 - Math.Abs(hue * 6 - 4);
            return new Color(Clamp(r), Clamp(g), Clamp(b));
        }

        private float Clamp(float value)
        {
            return Math.Max(0, Math.Min(1, value));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            foreach (var mesh in _sphereModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = Matrix.CreateTranslation(Vector3.Zero);
                    effect.View = Matrix.CreateLookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.1f, 100f);
                    effect.Texture = _rainbowTexture;
                    effect.TextureEnabled = true;
                }
                mesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
