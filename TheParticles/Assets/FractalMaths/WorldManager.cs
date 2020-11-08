using UnityEngine;
using System.Collections;

/**
 * This class builds the world.
 * 
 * The basic structure is that the world manage maintains a tree of geometry.
 * This trees are represented as a cube with the geometry that that cube would contain.
 * This cube may have sub cubes to increase the detail of that space.
 * 
 * To build the geometry, WorldManager uses fractal to render the geometry, that is,
 * identify which pieces are in the geometry.
 * To actually build the meshes, WorldManager uses geometrybuilder.
 * 
 * This geometry building process is built in parallel.
 * the struct ThreadData holds the information, for the geometry being built.
 * 
 * 
 * @author Joshua Alan Cook (jacook7@ncsu.edu
 **/
public class WorldManager : MonoBehaviour {
	/**
	 * The basic object to hold instantiated, fractal pieces
	 **/
	public Transform blank;
	/**
	 * The root for the worldBlocks tree.
	 **/
	public WorldBlock finalLord;
	/**
	 * The number of nodes per dimension.
	 **/
	private const int DIMENSION_LENGTH = 2;
	/**
	 * the index of the piece being rendered.
	 **/
	static int inRender;
	/**
	 * This is the fractal object to render the fractal.
	 **/
	public Fractal fractal;

	/**
	 * The struct to hold data for rendering threads.
	 **/
	struct ThreadData {
		/**
		 * This object builds the geometry.
		 * The only part that actually uses multithreading.
		 **/
		public geometryBuilder builder;
		/**
		 * this is used as an indicator to show if this thread is busy.
		 **/
		public int state;
		/**
		 * Which worldBlock to build.
		 **/
		public WorldBlock inProgress;

		/**
		 * Just initialize the thread data object. Namely,
		 * constuct the geometryBuilder builder.
		 **/
		public ThreadData (int dumby) {
			state = 0;
			builder = new geometryBuilder();
			inProgress = null;
		}
	};
	/**
	 * The array of different geometry builders.
	 **/
	private ThreadData[] thread;

	/**
	 * The worldBlocks to build for the next section.
	 **/
	private WorldBlock[] inQueue;
	/**
	 * The number of worldBlocks left to build in inQueue.
	 * Actually points to the number in inQueue to build.
	 * technically 0 is still considered one to add, so this is
	 * actually the number still to add - 1.
	 **/
	private int stillToAdd;
	/**
	 * The size of a single block of worldManager.
	 * That is, how detailed a the fractal object should render.
	 * A fractal renders a SectionSize X SectionSize X SectionSize cube.
	 **/
	public int SectionSize = 16;
	/**
	 * How many iterations the recursive formula in fractal should perform.
	 **/
	public int iterations = 32;

	/**
	 * World manager assumes cartesian space, but is not 
	 * deterministic as to what vectors the X Y and Z vectors
	 * correspond to in the fractal space.
	 * 
	 * WBegValue is the value of the smallest W in the fractal.
	 * WEndValue is the value of the largest W in the fractal.
	 **/
	public Vector4 XBegVal;
	public Vector4 YBegVal;
	public Vector4 ZBegVal;

	public Vector4 XEndVal;
	public Vector4 YEndVal;
	public Vector4 ZEndVal;

	/**
	 * The position to start the fractal at.
	 **/
	public Vector3 startPos;
	/**
	 * The size of the fractal.
	 **/
	public Vector3 range;

	/**
	 * The transform to base the geometry building by.
	 **/
	public Transform player;
	/**
	 * The maximum depth to build to.
	 **/
	public int depth;

	/**
	 * The polynomial to generate the fractal with.
	 * Length must be 5.
	 **/
	public float[] polynomial = new float[]{1, 0, 1, 0, 0};
	/**
	 * The randomizer to use to randomize this world.
	 **/
	public RuleRandomizer mixer;

	/**
	 * The hyper rules to use.
	 **/
	public HyperRules rules = HyperRules.Quaternionic;

	/**
	 * If a julia set is to be used, 0 for no, 1 for yes.
	 **/
	public float isJulia;

	/**
	 * The vector for the julia set to use, that is, its C.
	 **/
	public Vector4 juliaC;

	/**
	 * Called whenever WorldManager is created.
	 * Initializes inQueue, thread, and the root.
	 * Initlializes Fractal to follow these rules.
	 * 
	 * Specifically, finalLord is completely built and finalized, 
	 * then adds the first line of children to be built.
	 **/
	public void Start()
	{
		mixer.randomizeWorld(this);

		inQueue = new WorldBlock[DIMENSION_LENGTH*DIMENSION_LENGTH*DIMENSION_LENGTH];

		thread = new ThreadData[Mathf.Max(2, System.Environment.ProcessorCount-1)];

		for (int i = 0; i < thread.Length; i++) {
			thread[i] = new ThreadData(0);
		}

		fractal.SetUp(SectionSize, rules, polynomial, iterations, isJulia, juliaC);

		finalLord = new WorldBlock(new Vector3(0, 0, 0), new Vector3(1, 1, 1), null, this);
		finalLord.showBlock(Vector3.zero, 0, this);

		inRender = -1;
		stillToAdd = -1;
		Vector3 offSet = (finalLord.end - finalLord.start)/(SectionSize-2);
		fractal.StartRender(Vector4.Lerp(XBegVal, XEndVal, finalLord.start.x - offSet.x),
		                    Vector4.Lerp(XBegVal, XEndVal, finalLord.end.x + offSet.x),
		                    Vector4.Lerp(YBegVal, YEndVal, finalLord.start.y - offSet.y),
		                    Vector4.Lerp(YBegVal, YEndVal, finalLord.end.y + offSet.x),
		                    Vector4.Lerp(ZBegVal, ZEndVal, finalLord.start.z - offSet.z),
		                    Vector4.Lerp(ZBegVal, ZEndVal, finalLord.end.z + offSet.x));
		fractal.RetrieveRender();
		thread[0].builder.makeMeshFor(fractal.OutputBuffer, fractal.NormalBuffer, fractal.ConstantBuffer, SectionSize);
		inRender = -1;

		while (!thread[0].builder.isFinished) { }
		Mesh mesh = thread[0].builder.product();
		finalLord.finishBuild(mesh);

		showSection (Vector3.zero, 0);
		stillToAdd = inQueue.Length - 1;
	}

	/**
	 * Called every frame.
	 * This just calls upDateWorld with the play position.
	 **/
	public void Update()
	{
		upDateWorld(player.position);
	}

	/**
	 * Takes pos and builds the world based off it.
	 * In actuality, if something has been rendered, 
	 * passes it to be built by its appropriate thread.
	 * 
	 * Then, looks through threads, if a thread is not busy,
	 * it checks if something is currently rendering. If not,
	 * queries getNextSection to find the next worldBlock to render,
	 * and renders it.
	 * 
	 * If a builder has started building, and has finished,
	 * make a mesh and give it to the appropriate worldBlock.
	 * Finally, mark the thread complete.
	 * 
	 * @param pos the position to focus the build around.
	 **/
	public void upDateWorld(Vector3 pos)
	{
		if (inRender >= 0) { 
			fractal.RetrieveRender();
			thread[inRender].builder.makeMeshFor(fractal.OutputBuffer, fractal.NormalBuffer, fractal.ConstantBuffer, SectionSize);

			thread[inRender].state = 2;
			inRender = -1;
		}

		for (int i = 0; i < thread.Length; i++) {
			switch (thread[i].state) {
				case 0:
				if (inRender < 0) {

					thread[i].inProgress = getNextSection(pos);
					if (thread[i].inProgress != null) {
						inRender = i;
						Vector3 offSet = (thread[i].inProgress.end - thread[i].inProgress.start)/(SectionSize-2);
						fractal.StartRender(Vector4.Lerp(XBegVal, XEndVal, thread[i].inProgress.start.x - offSet.x),
						                    Vector4.Lerp(XBegVal, XEndVal, thread[i].inProgress.end.x + offSet.x),
						                    Vector4.Lerp(YBegVal, YEndVal, thread[i].inProgress.start.y - offSet.y),
						                    Vector4.Lerp(YBegVal, YEndVal, thread[i].inProgress.end.y + offSet.x),
						                    Vector4.Lerp(ZBegVal, ZEndVal, thread[i].inProgress.start.z - offSet.z),
						                    Vector4.Lerp(ZBegVal, ZEndVal, thread[i].inProgress.end.z + offSet.x));
						thread[i].state = 1;
					}
				}
				break;
				case 2:
				if (thread[i].builder.isFinished) {
					Mesh mesh = thread[i].builder.product();
					thread[i].inProgress.finishBuild(mesh);
					thread[i].state = 0;
					thread[i].inProgress.parent.showChildren();
					//debugMesh(mesh, thread[i].inProgress);
				}
				break;
			}
		}
	}

	/**
	 * Returns the appropriate next section to build based off
	 * of pos.
	 * 
	 * Actually, if stillToAdd is an index, just returns the next item
	 * inQueue. That is, the next item to finish the last section to add.
	 * 
	 * Otherwise, calls getBestSection with pos to refill the inQueue
	 * with appropriately most relevent section, based on pos.
	 * 
	 * @param the pos to add.
	 **/
	public WorldBlock getNextSection(Vector3 pos)
	{
		if (stillToAdd >= 0) {
			return inQueue[stillToAdd--];
		} else {
			getBestSection(pos);
		}
		if (stillToAdd >= 0) {
			return inQueue[stillToAdd--];
		} else return null;
	}

	/**
	 * Uses pos to find the best section and add it.
	 * Specifically, checks the nearby sections at 
	 * current level of detail, and adds the smallest depth.
	 * 
	 * To do this, it takes the position and depth and calls
	 * showSection with it.
	 * 
	 * @param pos the position to find the most pertenant piece for.
	 **/
	public void getBestSection(Vector3 pos) 
	{
		int curDepth = 1;
		Vector3 normalizedPos = getNormalizedPos(pos);
		if (0 < normalizedPos.x && normalizedPos.x < 1 && 
		    0 < normalizedPos.y && normalizedPos.y < 1 && 
		    0 < normalizedPos.z && normalizedPos.z < 1)
		{
			curDepth = Mathf.Min(finalLord.getDepth(normalizedPos), depth);
			Vector3 Offset = range/Mathf.Pow(DIMENSION_LENGTH, curDepth+1);

			Vector3 mostNeeded = normalizedPos;

			int minDepth = curDepth;

			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					for (int k = -1; k < 2; k++)
					{
						Vector3 curPos = getNormalizedPos(pos + new Vector3(Offset.x*i, Offset.y*j, Offset.z*k));
						if (0 < curPos.x && curPos.x < 1 && 
						    0 < curPos.y && curPos.y < 1 && 
						    0 < curPos.z && curPos.z < 1)
						{
							curDepth = finalLord.getDepth(curPos);
							if (curDepth < minDepth) {
								minDepth = curDepth;
								mostNeeded = curPos;
							}
						}
					}
				}
			}
			showSection(mostNeeded, minDepth + 1);
		}
	}

	/**
	 * calls show Block on finalLord, using depth and pos.
	 * Position must be normalized.
	 * 
	 * showBlock is what actually updates inQueue.
	 * 
	 * @param pos the pos parameter to use on showBlock
	 * @param depth the depth parameter to use on showBlock.
	 **/
	public void showSection(Vector3 pos, int depth)
	{
		finalLord.showBlock(pos, depth, this);
	}

	/**
	 * Takes a position and normalizes it.
	 * That is, positions at startPos are made 0, and positions
	 * at start + range made 1.
	 * 
	 * @param pos the position to normalize.
	 * @return a normalized version of pos.
	 **/
	public Vector3 getNormalizedPos(Vector3 pos)
	{
		Vector3 toRet = pos - startPos;
		return new Vector3(toRet.x/range.x, toRet.y/range.y, toRet.z/range.z);
	}

	/**
	 * Makes cubes around each vertex
	 **/
	public void debugMesh (Mesh source, WorldBlock loc)
	{
		foreach( Vector3 vert in source.vertices)
		{
			Vector3 AveragePos = (loc.start + loc.end)/2;
			AveragePos = startPos + new Vector3(range.x*AveragePos.x, range.y*AveragePos.y, range.z*AveragePos.z);
			
			Vector3 size = loc.end - loc.start;
			size = new Vector3(size.x*range.x, size.y*range.y, size.z*range.z);

			Transform newb = Instantiate(blank, AveragePos + new Vector3(size.x*vert.x, size.y*vert.y, size.z*vert.z), Quaternion.identity) as Transform;

			newb.localScale = size/(SectionSize*5);
		}
	}

	/**
	 * Most things in this group are meant to work through children,
	 * so programmers do not have to worry about making sure they get all the children to show, or not.
	 **/
	public class  WorldBlock
	{
		/**
		 * The game Object that will hold the mesh for this cube.
		 **/
		private GameObject myDetails;
		/**
		 * Whether the gameObject has been finished.
		 * That is, if myDetails has a mesh.
		 **/
		private bool isFinished;
		/**
		 * The subtrees of this worldBlock.
		 **/
		public WorldBlock[] children;
		/**
		 * The parent of this block
		 **/
		public WorldBlock parent;
		/**
		 * The bottom, back left corner, normalized.
		 **/
		public Vector3 start;
		/**
		 * The top, right, front corner, normalized
		 **/
		public Vector3 end;
		/**
		 * Constructs a new WorldBlock, with the given
		 * parameters given to worldBlocks parameters.
		 * 
		 * Most of this logic is to find the position and
		 * scale of myDetails.
		 **/
		public WorldBlock(Vector3 start, Vector3 end, WorldBlock parent, WorldManager man)
		{
			this.start = start;
			this.end = end;
			this.parent = parent;

			Vector3 AveragePos = (start + end)/2;
			AveragePos = man.startPos + new Vector3(man.range.x*AveragePos.x, man.range.y*AveragePos.y, man.range.z*AveragePos.z);
			Transform newbie = (Instantiate(man.blank, AveragePos, Quaternion.identity) as Transform) as Transform;
			
			Vector3 size = end - start;
			size = new Vector3(size.x*man.range.x, size.y*man.range.y, size.z*man.range.z);
			newbie.transform.localScale = size;

			myDetails = newbie.gameObject;
			myDetails.SetActive(false);

			isFinished = false;
		}
		/**
		 * Gives a mesh to myDetails and sets isFinished to true.
		 * 
		 * @param me the mesh to give to myDetails.
		 **/
		public void finishBuild(Mesh me)
		{
			myDetails.GetComponent<MeshFilter>().mesh = me;
			isFinished = true;
		}

		/**
		 * Instantiates the children of this WorldBlock.
		 * the manager to tell to build the meshes of the children.
		 * 
		 * @param man the manager to build the children with.
		 **/
		public void buildChildren(WorldManager man)
		{
			children = new WorldBlock[DIMENSION_LENGTH*DIMENSION_LENGTH*DIMENSION_LENGTH];

			Vector3 range = (1f/DIMENSION_LENGTH)*(end - start);
			for (int i = 0; i < DIMENSION_LENGTH; i++)
			{
				for (int j = 0; j < DIMENSION_LENGTH; j++)
				{
					for (int k = 0; k < DIMENSION_LENGTH; k++)
					{
						Vector3 newStart = start + new Vector3(range.x*i, range.y*j, range.z*k);
						Vector3 newEnd = newStart + range;
						int index = (k*DIMENSION_LENGTH + j)*DIMENSION_LENGTH + i;
						children[index] = new WorldBlock(newStart, newEnd, this, man);
						man.inQueue[index] = children[index];
					}
				}
			}

			man.stillToAdd = DIMENSION_LENGTH*DIMENSION_LENGTH*DIMENSION_LENGTH - 1;
		}

		/**
		 * Gets the maximum depth this subtree has at pos.
		 * 
		 * @param pos, the position to find max depth at.
		 **/
		public int getDepth(Vector3 pos)
		{
			if (children == null)
			{
				if (isFinished) return 1;
				else return 0;
			}
			else
			{
				pos *= DIMENSION_LENGTH;
				int x = Mathf.FloorToInt(pos.x);
				int y = Mathf.FloorToInt(pos.y);
				int z = Mathf.FloorToInt(pos.z);
				int index = (z*DIMENSION_LENGTH + y)*DIMENSION_LENGTH + x;
				return children[index].getDepth(pos - new Vector3(x, y, z)) + 1;
			}
		}

		/**
		 * if the block at depth is not build, it builds it.
		 * 
		 * @param pos the position tp show the block at.
		 * @param maxDepth, the depth to show at.
		 * @param man the manager to tell to build me.
		 **/
		public void showBlock(Vector3 pos, int maxDepth, WorldManager man)
		{
			if (maxDepth <= 0)
			{
				if (children != null)
				{
					//showChildren();
				}
				else buildChildren(man);
			}
			else
			{
				if (children == null) buildChildren(man);
				else //if (isFinished)
				{
					pos *= DIMENSION_LENGTH;
					int x = Mathf.FloorToInt(pos.x);
					int y = Mathf.FloorToInt(pos.y);
					int z = Mathf.FloorToInt(pos.z);
					int index = (z*DIMENSION_LENGTH + y)*DIMENSION_LENGTH + x;
					children[index].showBlock(pos - new Vector3(x, y, z), maxDepth-1, man);
				}
			}
		}

		/**
		 * finds whether all the children of this are complete.
		 * That is, if the all have meshes.
		 * 
		 * @return whether the children are complete
		 **/
		bool childComplete()
		{
			if (children == null) return false;
			for (int i = 0; i < children.Length; i++) {
				if (!children[i].isFinished) return false;
			}
			return true;
		}

		/**
		 * If shildComplete,
		 * sets in active, and shows children.
		 * 
		 * That is, on all children, takeFocusFromChildren is called
		 **/
		public void showChildren()
		{
			if (childComplete()) {
				myDetails.SetActive(false);
				for (int i = 0; i < children.Length; i++)
				{
					children[i].takeFocusFromChildren();
				}
			}
		}
		/**
		 * Sets all children to not visible and self to visible.
		 * 
		 * That is, sets myDetails to active,
		 * and calls totalUnFocus on all children
		 **/
		public void takeFocusFromChildren()
		{
			if (!myDetails.activeSelf)
			{
				myDetails.SetActive(true);
				if (children != null)
				{
					for (int i = 0; i < children.Length; i++)
					{
						if (children[i] != null)
						{
							children[i].totalUnFocus();
						}
					}
				}
			}
		}

		/**
		 * sets myDetails to unactive, and calls total unfocus
		 * on all children.
		 **/
		public void totalUnFocus()
		{
			if (myDetails != null && myDetails.activeSelf)
			{
				myDetails.SetActive(false);
			}
			else
			{
				if (children != null)
				{
					for (int i = 0; i < children.Length; i++)
					{
						if (children[i] != null)
						{
							children[i].totalUnFocus();
						}
					}
				}
			}
		}

		/**
		 * Removes this game object, and calls kill children
		 **/
		public void Destroy()
		{
			KillChildren();
			UnityEngine.Object.Destroy(myDetails);
			parent = null;
		}

		/**
		 * Removes the children from this object. Only do this when we need more Ram
		 **/
		public void KillChildren()
		{
			for (int i = 0; i < children.Length; i++)
			{
				if (children[i] != null)
				{
					children[i].Destroy();
					children[i] = null;
				}
			}
		}
	}
}
