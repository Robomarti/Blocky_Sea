#ifndef ADDITIONAL_LIGHT_INCLUDED
#define ADDITIONAL_LIGHT_INCLUDED

void GetIteratedSine_float(float Iterations, float Amplitude, float Persistence, float Frequency, float Lacunarity, float Input_X, float Input_Z, out float Y_Position) {
    float sinCos = 0;

    for (int i = 0; i < Iterations; i++) {
        float result_x = exp(sin(Input_X * Frequency)-1);
        float result_z = exp(cos(Input_Z * Frequency)-1);
        sinCos += Amplitude * (result_x + result_z);
        Amplitude *= Persistence;
        Frequency *= Lacunarity;
    }

    Y_Position = sinCos;
}

#endif