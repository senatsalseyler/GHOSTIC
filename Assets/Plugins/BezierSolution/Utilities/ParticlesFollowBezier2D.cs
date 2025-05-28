using System.Collections.Generic;
using UnityEngine;

namespace BezierSolution
{
	[AddComponentMenu( "Bezier Solution/Particles Follow Bezier 2D" )]
	[HelpURL( "https://github.com/yasirkula/UnityBezierSolution" )]
	[RequireComponent( typeof( ParticleSystem ) )]
	[ExecuteInEditMode]
	public class ParticlesFollowBezier2D : MonoBehaviour
	{
		private const int MAX_PARTICLE_COUNT = 25000;

		public enum FollowMode { Relaxed, Strict };

		[Header("Bezier Settings")]
		public BezierSpline spline;
		public FollowMode followMode = FollowMode.Relaxed;
		
		[Header("2D Settings")]
		[Tooltip("Z position for particles (important for 2D visibility)")]
		public float zPosition = 0f;
		[Tooltip("Force particles to stay on Z plane")]
		public bool lockZPosition = true;
		
		[Header("Debug")]
		[Tooltip("Show debug info in console")]
		public bool debugMode = false;

		private Transform cachedTransform;
		private ParticleSystem cachedPS;
		private ParticleSystem.MainModule cachedMainModule;
		private ParticleSystem.Particle[] particles;
		private List<Vector4> particleData;

		private void Awake()
		{
			cachedTransform = transform;
			cachedPS = GetComponent<ParticleSystem>();

			cachedMainModule = cachedPS.main;
			particles = new ParticleSystem.Particle[cachedMainModule.maxParticles];

			if( followMode == FollowMode.Relaxed )
				particleData = new List<Vector4>( particles.Length );
				
			// Ensure particle system is set up for 2D
			Setup2DParticleSystem();
		}

		private void Setup2DParticleSystem()
		{
			if (cachedPS == null) return;
			
			// Set simulation space to world for better 2D control
			var main = cachedPS.main;
			if (main.simulationSpace == ParticleSystemSimulationSpace.Local)
			{
				main.simulationSpace = ParticleSystemSimulationSpace.World;
				if (debugMode) Debug.Log("Changed particle simulation space to World for 2D compatibility");
			}
			
			// Ensure shape module doesn't interfere
			var shape = cachedPS.shape;
			if (shape.enabled)
			{
				shape.enabled = false;
				if (debugMode) Debug.Log("Disabled shape module to prevent interference with bezier path");
			}
			
			// Set velocity over lifetime to zero to prevent unwanted movement
			var velocityOverLifetime = cachedPS.velocityOverLifetime;
			if (velocityOverLifetime.enabled)
			{
				velocityOverLifetime.enabled = false;
				if (debugMode) Debug.Log("Disabled velocity over lifetime to prevent interference with bezier path");
			}
		}

#if UNITY_EDITOR
		private void OnEnable()
		{
			Awake();
		}
#endif

#if UNITY_EDITOR
		private void LateUpdate()
		{
			if( !UnityEditor.EditorApplication.isPlaying )
				FixedUpdate();
		}
#endif

		private void FixedUpdate()
		{
			if( spline == null || cachedPS == null )
			{
				if (debugMode && spline == null) Debug.LogWarning("Spline is null!");
				if (debugMode && cachedPS == null) Debug.LogWarning("ParticleSystem is null!");
				return;
			}

			if( particles.Length < cachedMainModule.maxParticles && particles.Length < MAX_PARTICLE_COUNT )
				System.Array.Resize( ref particles, Mathf.Min( cachedMainModule.maxParticles, MAX_PARTICLE_COUNT ) );

			bool isLocalSpace = cachedMainModule.simulationSpace != ParticleSystemSimulationSpace.World;
			int aliveParticles = cachedPS.GetParticles( particles );
			
			if (debugMode && aliveParticles > 0)
				Debug.Log($"Processing {aliveParticles} alive particles");

			Vector3 initialPoint = spline.GetPoint( 0f );
			
			if( followMode == FollowMode.Relaxed )
			{
				if( particleData == null )
					particleData = new List<Vector4>( particles.Length );

				cachedPS.GetCustomParticleData( particleData, ParticleSystemCustomData.Custom1 );

				// Credit: https://forum.unity3d.com/threads/access-to-the-particle-system-lifecycle-events.328918/#post-2295977
				for( int i = 0; i < aliveParticles; i++ )
				{
					Vector4 particleDat = particleData[i];
					float normalizedTime = 1f - ( particles[i].remainingLifetime / particles[i].startLifetime );
					Vector3 point = spline.GetPoint( normalizedTime );
					
					if( !isLocalSpace )
						point = cachedTransform.TransformPoint( point - initialPoint );
					else
						point = point - initialPoint;

					// Force Z position for 2D
					if (lockZPosition)
						point.z = zPosition;

					// Move particles alongside the spline
					if( particleDat.w != 0f )
						particles[i].position += point - (Vector3) particleDat;
					else
						particles[i].position = point; // First frame positioning

					particleDat = point;
					particleDat.w = 1f;
					particleData[i] = particleDat;
				}

				cachedPS.SetCustomParticleData( particleData, ParticleSystemCustomData.Custom1 );
			}
			else
			{
				for( int i = 0; i < aliveParticles; i++ )
				{
					float normalizedTime = 1f - ( particles[i].remainingLifetime / particles[i].startLifetime );
					Vector3 point = spline.GetPoint( normalizedTime ) - initialPoint;
					
					if( !isLocalSpace )
						point = cachedTransform.TransformPoint( point );

					// Force Z position for 2D
					if (lockZPosition)
						point.z = zPosition;

					particles[i].position = point;
				}
			}

			cachedPS.SetParticles( particles, aliveParticles );
		}
		
		// Helper method to adjust Z position at runtime
		public void SetZPosition(float newZ)
		{
			zPosition = newZ;
		}
		
		// Method to force particle system refresh
		[ContextMenu("Refresh Particle System")]
		public void RefreshParticleSystem()
		{
			Setup2DParticleSystem();
			if (debugMode) Debug.Log("Particle system refreshed for 2D");
		}
	}
}