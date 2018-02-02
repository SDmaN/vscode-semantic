raw
#include <cmath>
end

module Math

public fun (val int a) : int Abs
    int result = 0

    raw
    result = abs(a);
    end

    return result
end

public fun (val real a) : real FAbs
    real result = 0

    raw
    result = fabs(a);
    end

    return result
end

public fun (val real a, val real b) : real Pow
    real result = 0

    raw
    result = pow(a, b);
    end

    return result
end

public fun (val real a) : real Log
    real result = 0

    raw
    result = log10(a);
    end

    return result
end

public fun (val real a) : real Ln
    real result = 0

    raw
    result = log(a);
    end

    return result
end

public fun (val real a) : real Exp
    real result = 0

    raw
    result = exp(a);
    end

    return result
end

public fun (val real a) : real Sqrt
    real result = 0

    raw
    result = sqrt(a);
    end

    return result
end

public fun (val real a) : real Sin
    real result = 0

    raw
    result = sin(a);
    end

    return result
end

public fun (val real a) : real Cos
    real result = 0

    raw
    result = cos(a);
    end

    return result
end

start
end