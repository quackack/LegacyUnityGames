using UnityEngine;
using System.Collections;

public interface Formula{

	HyperComplex function(HyperComplex Z, HyperComplex C);
}
