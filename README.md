2D Unity Project - Sudoku

Initializes a sudoku board utilizing Unity's UI system
Generates a puzzle using DLX (Dancing Links) Algorithm

My timeline/notes

1. Create the grid
	1. 9x9 Grid with 3x3 blocks
	2. WHY THE FUCK IS MAKING A GRID SO FUCKING HARD
		1. Created a grid using UI
2. Difficulty
	1. Amount of clues given and make sure it's solvable
3. General
	1. Generate numbers within the squares
	2. Make sure every column and row numbers are unique
	3. Able to insert numbers in the squares
	4. Able to create notes and possible numbers for squares
4. Want to make a desktop app --- borderless like a popup


4/14/25 :

	1. Be able to add numbers
	2. Create a sudoku logic where only unique numbers exist for column, row, and block
	3. Add user input

4/19/25 :

	1. Added Numbers
	2. Figure out how to destroy new text
	3. Numbers don't show up at first spawn
	4. Able to store the rows and columns to spawn in center of cells

4/21/25

	1. Full generated grid with unique numbers
	2. Has a solver
	3. Need to create UI to hit play - solve - reset
	4. Stores numbers as rows, columns, and blocks*
	5. Need to remove numbers to create puzzles

4/22/25

	1. Generates a puzzle now using DLX algorithm
	2. Added user input 
	3. Added a solve button
	4. Made UI scale constant --- need to test on other resolutions
	5. Need to make if => (solved) ? complete : wrong solution

4/23/25

	1. Created a checker of the solved board
		1. Maybe figure out other ways to check if board is solved
		2. Maybe make a mistake system
	2. Make a notes system
	3. Need to make a UI start system
	4. Need to humanize the game
