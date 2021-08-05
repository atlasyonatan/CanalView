namespace PuzzleSolving
{
    public static class BitWiseOperators
    {
        public static uint RotateLeft(uint input, int n, int bitSize)
        {
            n %= bitSize;
            var rightMask = (1u << (bitSize - n)) - 1;
            var leftMask = (1u << bitSize) - 1 - rightMask;
            var right = input & rightMask;
            var left = input & leftMask;
            return (right << n) + (left >> (bitSize - n));
        }
    }
}
