import pygame
import random

# Initialize pygame
pygame.init()

# Game Constants
WIDTH, HEIGHT = 800, 600
PLAYER_SPEED = 5
OBSTACLE_SPEED = 3
EXTRA_LIFE_CHANCE = 0.05

# Colors
WHITE = (255, 255, 255)
RED = (255, 0, 0)
GREEN = (0, 255, 0)
BLUE = (0, 0, 255)
YELLOW = (255, 255, 0)

# Load Images
player_img = pygame.image.load("player.png")
obstacle_img = pygame.image.load("obstacle.png")
extra_life_img = pygame.image.load("extra_life.png")
tsunami_img = pygame.image.load("tsunami.png")

# Scale Images
player_img = pygame.transform.scale(player_img, (50, 50))
obstacle_img = pygame.transform.scale(obstacle_img, (50, 50))
extra_life_img = pygame.transform.scale(extra_life_img, (40, 40))
tsunami_img = pygame.transform.scale(tsunami_img, (100, HEIGHT))

# Game Setup
screen = pygame.display.set_mode((WIDTH, HEIGHT))
clock = pygame.time.Clock()
running = True

# Player
player = pygame.Rect(100, HEIGHT // 2, 50, 50)
player_lives = 3

# Obstacles
obstacles = []
extra_lives = []

def spawn_obstacle():
    y_pos = random.randint(50, HEIGHT - 50)
    obstacles.append(pygame.Rect(WIDTH, y_pos, 50, 50))

def spawn_extra_life():
    y_pos = random.randint(50, HEIGHT - 50)
    extra_lives.append(pygame.Rect(WIDTH, y_pos, 40, 40))

while running:
    screen.fill(WHITE)
    screen.blit(tsunami_img, (0, 0))

    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False

    keys = pygame.key.get_pressed()
    if keys[pygame.K_UP] and player.top > 0:
        player.y -= PLAYER_SPEED
    if keys[pygame.K_DOWN] and player.bottom < HEIGHT:
        player.y += PLAYER_SPEED

    # Spawn obstacles
    if random.random() < 0.02:
        spawn_obstacle()
    
    # Spawn extra life
    if random.random() < EXTRA_LIFE_CHANCE:
        spawn_extra_life()
    
    # Move obstacles
    for obstacle in obstacles[:]:
        obstacle.x -= OBSTACLE_SPEED
        if obstacle.right < 0:
            obstacles.remove(obstacle)
        if player.colliderect(obstacle):
            player_lives -= 1
            obstacles.remove(obstacle)

    # Move extra life
    for extra_life in extra_lives[:]:
        extra_life.x -= OBSTACLE_SPEED
        if extra_life.right < 0:
            extra_lives.remove(extra_life)
        if player.colliderect(extra_life):
            player_lives += 1
            extra_lives.remove(extra_life)

    # Draw player
    screen.blit(player_img, (player.x, player.y))
    
    # Draw obstacles
    for obstacle in obstacles:
        screen.blit(obstacle_img, (obstacle.x, obstacle.y))
    
    # Draw extra lives
    for extra_life in extra_lives:
        screen.blit(extra_life_img, (extra_life.x, extra_life.y))
    
    # Display Lives
    font = pygame.font.Font(None, 36)
    lives_text = font.render(f"Lives: {player_lives}", True, RED)
    screen.blit(lives_text, (10, 10))
    
    # Check game over
    if player_lives <= 0:
        running = False

    pygame.display.flip()
    clock.tick(30)

pygame.quit()
