using UnityEngine;
using System.Collections;

namespace Kubility
{
	public interface ContentFactoryInterface
	{
		Content Create(UIType type);
	}

}


