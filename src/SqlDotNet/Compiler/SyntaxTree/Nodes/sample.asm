// Statement: insert into "tasdasd" (a, b, c) values ('a', 123, '123123123')
@siql v1 // Language and version

ldc.str a
ldc.i4 123
ldc.str '123123123'

// Call internal method
call._insert_into "tasdasd" (a, b, c)


// ------------------------------------------------------------------------
// Statement: select A from B where 1 = 1
ocur.tbl cur0		// Open a new cursor and name it cur0. Syntx ocur.<source-type> cur0
curout.cur0 (A)		// Defines the result of the cursor (cursor out). Syntaxt curout (list of result columns)
cursrc.cur0 B		// Set the cursor source. Syntax cursrc.<name> Source-Name
oresset res0		// Open/create a new result-set
filter cur0
{
	
}
fresset.res0 (cur0)	// Fill the result-set res0 using cursor cur0
{
	crow res0		// Create a new row in the result-set 0
	lcol.cur0 A		// Load column value A to the stack
	ptnextr			// Pop the value form the stack to the next column
}


//Statement: select func(1, 2)
ocur.none cur0			// Open a new cursor and name it cur0. Syntx ocur.<source-type> cur0
curout.cur0 (__col0)	// Defines the result of the cursor (cursor out). Syntaxt curout (list of result columns)
oresset res0			// Open/create a new result-set
fresset.res0 (cur0)		// Fill the result-set res0 using cursor cur0
{
	crow res0				// Create a new row in the result-set 0
	ldc.i4 1
	ldc.i4 2

	call.f func(arg0, arg1)	// Call internal method
	ptnextr					// Pop the value form the stack to the next column
}