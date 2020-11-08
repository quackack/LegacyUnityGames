using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block {
    private Transform block;

    Block[,,] children;
    public Vector3 start;
    public Vector3 size;
    state current = state.BeingBuilt;
    Block parent;

    private enum state
    {
        BeingBuilt,
        Built,
        WithChildren
    }

    public void finishBuild(Transform block, Vector3 realStart, Vector3 realSize)
    {
        this.block = block;
        block.localPosition = realStart;
        block.localScale = realSize;

        current = state.Built;
        if (parent != null)
        {
            parent.hasChlidren();
        }
        else
        {
            turnOn();
        }
    }

    private bool hasChlidren()
    {
        switch (current)
        {
            default:
            case state.BeingBuilt:
                return false;
            case state.WithChildren:
                return true;
            case state.Built:
                foreach (Block child in children)
                {
                    if (child.current != state.Built && child.current != state.WithChildren)
                    {
                        return false;
                    }
                }
                foreach (Block child in children)
                {
                    child.turnOn();
                }
                turnOff();
                current = state.WithChildren;
                return true;
        }
    }

    public Block(Vector3 start, Vector3 size, Block parent)
    {
        this.start = start;
        this.size = size;
        this.parent = parent;
    }

    public Block getInBlock(Vector3 pos)
    {
        if (current != state.WithChildren)
        {
            return this;
        }
        Vector3 relPos = (pos - start) * 2;
        int xInd, yInd, zInd;
        if (relPos.x > size.x) xInd = 1;
        else xInd = 0;
        if (relPos.y > size.y) yInd = 1;
        else yInd = 0;
        if (relPos.z > size.z) zInd = 1;
        else zInd = 0;
        return children[xInd, yInd, zInd].getInBlock(pos);
    }

    public void buildInMe(Queue<Block> tasks)
    {
        if (children != null)
        {
            return;
        }
        children = new Block[2, 2, 2];
        for (int i = 0; i < 2; i++) for (int j = 0; j < 2; j++) for (int k = 0; k < 2; k++)
                {
                    children[i, j, k] = new Block(this.start + Block.parMult(this.size, new Vector3(i, j, k) * 0.5f), this.size * 0.5f, this);
                }
        foreach (Block child in children)
        {
            tasks.Enqueue(child);
        }
    }

    public static Vector3 parMult(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

	public void turnOn()
    {
        block.gameObject.SetActive(true);
    }

    public void turnOff()
    {
        block.gameObject.SetActive(false);
    }

    public void delete()
    {
        MonoBehaviour.Destroy(block.gameObject);
    }
}
