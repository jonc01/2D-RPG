- If using an object pool, set "Stop Action" to "Callback". 

1) Use ParticlePoolHelper.cs (attach script to particle prefab) disables the object when the particle system is done, disables the object and pools it.

2)ObjectPoolerList.cs (attach script to ObjectPool GameObject) already enables objects


- Otherwise, make sure it is set to "Destroy".