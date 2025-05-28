using UnityEditor;
using UnityEngine;

namespace BezierSolution.Extras
{
	// This class is used for resetting the particle system attached to a ParticlesFollowBezier
	// component when it is selected. Otherwise, particles move in a chaotic way for a while
	[CustomEditor( typeof( ParticlesFollowBezier2D ) )]
	[CanEditMultipleObjects]
	public class ParticlesFollowBezierEditor2D : Editor
	{
		private int particlesReset;

		private void OnEnable()
		{
			particlesReset = 3;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if( Application.isPlaying )
				return;

			if( particlesReset > 0 && --particlesReset == 0 )
			{
				foreach( Object target in targets )
				{
					ResetParticles( ( (ParticlesFollowBezier2D) target ).GetComponentsInParent<ParticlesFollowBezier2D>() );
					ResetParticles( ( (ParticlesFollowBezier2D) target ).GetComponentsInChildren<ParticlesFollowBezier2D>() );
				}
			}
		}

		private void ResetParticles( ParticlesFollowBezier2D[] targets )
		{
			foreach( ParticlesFollowBezier2D target in targets )
			{
				ParticleSystem particleSystem = target.GetComponent<ParticleSystem>();
				if( target.spline != null && particleSystem != null && target.enabled )
					particleSystem.Clear();
			}
		}
	}
}