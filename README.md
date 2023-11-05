# AimGame
 
This README outlines the pseudo-implementation of how the AI both within this project and within [Map Game](https://github.com/Lucas-Rose/MapGame) use the analytics gathered by playtesters in the Aim Training Scene to simulate realistic mouse movements.

## Offset Generation
1. To begin aiming towards a target, the StartAiming(Transform) method is called, passing the transform of the target we are wishing to aim at as it will contain it's position among other things. 
2. The number of times that the AI Camera should miss uses a gradient parameter which increases logarithmically with regard to how far away the target is from the perspective of the camera on the z axis.
   - The number of times it should miss is calculated using this gradient multiplied by the targets distance. For every multiple of one that this value is evaluated to a miss is added and the remainder fraction is interpreted as a chance of receiving another miss.
3. GenerateShotVectors() is then called:
   - Using a parameter of inaccuracy on both the x and y axis, points around the target will be randomly generated within the constraints around the targets position and added to a list.

## Reaction + Aiming
1. After a specified reaction time period, the AI will then begin moving through the list of waypoints generated during the Offset Generation Phase
   - Note that it can be specified if the AI actually sends out a Raycast during every miss, however if the miss gradient is very small it will be hard to tell how many misses were generated.
2. Dependent upon how far away the target is, an **Aiming Gradient** is used to specify how quickly the camera should rotate between the generated miss points
   - Naturally, aiming / flick speed will increase exponentially when the rotation vector between the AI's current rotation and the target position is very small.
3. After successfully moving through all of the miss points within the list, a raycast will be sent out, if it hasn't been sent already, to hit the target and proceed with the specified outcome of the game. 
