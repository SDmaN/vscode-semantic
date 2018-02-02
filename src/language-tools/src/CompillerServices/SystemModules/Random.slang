raw
#include <random>
#include <chrono>
end

module Random

public fun (val int a, val int b) : int Integer
	int result

	raw
	unsigned seed = std::chrono::system_clock::now().time_since_epoch().count();
	std::default_random_engine generator(seed);
	std::uniform_int_distribution<int64_t> distribution(a, b);
	result = distribution(generator);
	end
	
	return result
end

public fun () : real Real
	real result

	raw
	unsigned seed = std::chrono::system_clock::now().time_since_epoch().count();
	std::default_random_engine generator(seed);
	std::uniform_real_distribution<double> distribution(0.0, 1.0);
	result = distribution(generator);
	end

	return result
end

start
end