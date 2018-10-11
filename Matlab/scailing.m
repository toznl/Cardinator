x1= sym('x1')
y1= sym('y1')
z1= sym('z1')

x2= sym('x2')
y2= sym('y2')
z2= sym('z2')

syms ('a11', 'a12', 'a13', 'a21', 'a22', 'a23', 'a31', 'a32', 'a33')

X1 = [x1 ;y1; z1]
X2 = [x2 ;y2; z2]

A = [a11, a12, a13 ;
    a21, a22, a23;
    a31,a32, a33]

    
A*X1

