using UnityEngine;
using System.Collections.Generic;

public static class IgnoreBulletCollision
{
    // Cache all branch colliders once
    private static List<Collider2D> branchColliders;

    /// <summary>
    /// Initializes the branch colliders cache.
    /// Call this once at scene start or before firing any projectiles.
    /// </summary>
    [System.Obsolete]

    public static void Initialize()
    {
        branchColliders = new List<Collider2D>();

        // Find all BranchMarker objects (including inactive ones)
        IgnoredObjectsMarker[] branches = Object.FindObjectsOfType<IgnoredObjectsMarker>(true);

        foreach (IgnoredObjectsMarker branch in branches)
        {
            // Add all colliders on this object and its children
            Collider2D[] colliders = branch.GetComponentsInChildren<Collider2D>(true);
            branchColliders.AddRange(colliders);
        }
    }

    /// <summary>
    /// Make the given projectile ignore collisions with all branches.
    /// </summary>
    [System.Obsolete]

    public static void Apply(Collider2D projectileCol)
    {
        if (projectileCol == null) return;
        if (branchColliders == null) Initialize(); // auto-init if needed

        foreach (Collider2D branchCol in branchColliders)
        {
            if (branchCol != null)
                Physics2D.IgnoreCollision(projectileCol, branchCol, true);
        }
    }
}
