using UnityEngine;

public static class IgnoreBranchCollision
{
    public static void Apply(Collider2D projectileCol)
    {
        if (projectileCol == null) return;

        
        var branches = Object.FindObjectsOfType<BranchMarker>(true);

        foreach (var b in branches)
        {
           
            var branchCols = b.GetComponentsInChildren<Collider2D>(true);

            foreach (var branchCol in branchCols)
            {
                if (branchCol != null)
                    Physics2D.IgnoreCollision(projectileCol, branchCol, true);
            }
        }
    }
}
