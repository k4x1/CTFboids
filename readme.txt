
# Boids Game

## Overview
This game is a team-based capture-the-flag game where AI-controlled boids (agents) navigate the environment to achieve various goals such as capturing flags, avoiding obstacles, and rescuing jailed teammates. The game is implemented using Unity and C#.

## Controls
- **Left Mouse Button**: Select the closest boid to the mouse cursor and make it playable. Clicking again will deselect the boid.
- **Mouse Movement**: Move the mouse to control the position of the boid manager, which influences the boids' behavior.

## AI Techniques

### Boid Behavior
The AI for the boids is based on the Boids algorithm, which simulates the flocking behavior of birds. The main components of this behavior are:

1. **Separation (Avoidance)**: Boids steer to avoid crowding local flockmates.
2. **Alignment (Destination)**: Boids steer towards the average heading of local flockmates.
3. **Cohesion**: Boids steer to move towards the average position of local flockmates.

### Goal-Oriented Action Planning (GOAP)
Each boid has a set of goals with associated weights that determine its behavior. The goals include:
- **Capture Flag**: Move towards and capture the enemy flag.
- **Avoid Obstacles**: Steer away from obstacles to avoid collisions.
- **Rescue Teammates**: Move towards jailed teammates to rescue them.
- **Wander**: Move randomly when no other goals are prioritized.

### Decision Making
Boids make decisions based on the weights of their goals. The weights are dynamically updated based on the game state:
- **Flag Capture**: High priority if the boid can go to the enemy side and the flag is available.
- **Avoidance**: High priority when obstacles are detected within a certain range.
- **Rescue**: High priority if teammates are jailed and the boid can rescue them.

### Movement and Steering
Boids use a combination of raycasting and vector calculations to determine their movement:
- **Raycasting**: Detects obstacles and other boids within a certain range.
- **Vector Calculations**: Combines avoidance and destination vectors to update the boid's velocity and direction.

### Team-Specific Behavior
Boids have team-specific behaviors:
- **Taggable**: Boids can be tagged (jailed) by enemy boids if they are on the enemy side.
- **Defenders**: Some boids are designated as defenders and cannot go to the enemy side.

## How It Works Together
1. **Initialization**: Boids are initialized with random positions and assigned to teams.
2. **Goal Registration**: Each boid registers itself and its goals with the boid manager.
3. **Movement Processing**: In each update cycle, boids process their movement by:
   - Checking team-specific conditions.
   - Performing raycasting to detect obstacles.
   - Calculating destination and avoidance vectors.
   - Updating their velocity and direction based on the combined vectors.
4. **Goal Processing**: Boids evaluate their goals and update their weights based on the current game state.
5. **Interaction**: Boids interact with each other and the environment, such as capturing flags, avoiding obstacles, and rescuing teammates.

## Conclusion
The combination of Boids algorithm, GOAP, and team-specific behaviors creates a dynamic and engaging AI system for the game. The boids work together to achieve their goals, resulting in a challenging and fun gameplay experience.

## Future Improvements
- **Enhanced AI**: Implement more sophisticated AI techniques such as machine learning for adaptive behavior.
- **Multiplayer Support**: Add support for multiple players to control different boids.
- **Improved Graphics**: Enhance the visual representation of the game for a better user experience.

## Credits
- **Developer**: Kazuo Reis de Andrade


