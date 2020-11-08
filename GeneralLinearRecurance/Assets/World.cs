using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    public Transform start;
    public CallIteration iterator;
    public Vector3 size = new Vector3(100, 100, 100);
    public IterationSetter itSetter;
    public Transform player;

    private Block root;
    
    public Queue<builder> readyQueue = new Queue<builder>();
    public Queue<builder> finishedQueue = new Queue<builder>();

    public Queue<Block> immediateBuild = new Queue<Block>();

    // Use this for initialization
    void Start()
    {
        itSetter.setRules(iterator);
        root = new Block(Vector3.zero, new Vector3(1, 1, 1), null);
        immediateBuild.Enqueue(root);
        for (int i = 0; i < 2; i++)
        {
            readyQueue.Enqueue(new builder(iterator.dimension, finishedQueue, start));
        }
    }

    public void Update()
    {
        while (finishedQueue.Count > 0)
        {
            builder b;
            lock (finishedQueue)
            {
                b = finishedQueue.Dequeue();
            }
            b.finishJob();
            readyQueue.Enqueue(b);
        }
        if (immediateBuild.Count == 0)
        {
            getNextBuild();
        }
        if (immediateBuild.Count > 0 && readyQueue.Count > 0)
        {
            readyQueue.Dequeue().startJob(iterator, immediateBuild.Dequeue(), size);
        }
    }

    private void getNextBuild()
    {
        Vector3 relativePos = parDiv(player.transform.position, size);
        Block toBuild =  root.getInBlock(relativePos);
        toBuild.buildInMe(immediateBuild);
    }

    public Vector3 parDiv(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }
}
