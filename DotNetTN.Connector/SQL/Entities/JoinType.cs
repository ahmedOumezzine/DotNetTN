namespace DotNetTN.Connector.SQL.Entities
{
    public enum JoinType
    {
        Inner = 0,
        Left = 1,
        leftWithoutIntersection = 11,
        Right = 2,
        RightWithoutIntersection = 22,
        FULL = 3
    }
}