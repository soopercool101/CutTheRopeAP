using CutTheRope.iframework.helpers;

namespace CutTheRope.iframework.visual
{
	internal class ScalableMultiParticles : MultiParticles
	{
		public override void initParticle(ref Particle particle)
		{
			Image image = imageGrid;
			int num = MathHelper.RND(image.texture.quadsCount - 1);
			Quad2D qt = image.texture.quads[num];
			Quad3D qv = Quad3D.MakeQuad3D(0f, 0f, 0f, 0f, 0f);
			Rectangle rectangle = image.texture.quadRects[num];
			drawer.setTextureQuadatVertexQuadatIndex(qt, qv, particleCount);
			base.initParticle(ref particle);
			particle.width = rectangle.w;
			particle.height = rectangle.h;
		}
	}
}
