using k8s.Models;

public static class ExtensionMethods
{
    public static V1Taint Parse(this V1Taint taint, string taintString) {
        var t1 = taintString.Split(":");
        if (t1.Length != 2)
            throw new ArgumentOutOfRangeException("String is not a valid taint");
        taint.Effect = t1[1];
        var t2 = t1[0].Split("=");
        if (t2.Length > 1) 
            taint.Value = t2[1];
        taint.Key = t2[0];
        return taint;
    }
}