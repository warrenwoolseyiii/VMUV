// Common.h
// #defined used to simplify porting from CSharp to C++

#pragma once

#include <iostream>

using namespace std;

#define Int16 short
#define byte unsigned char

static bool AssertAreEqual(int a, int b, char* name, int id) {
	if (a != b) {
		cout << "unit test " << name << id << " FAILED\n";
	}
	return (a == b);
}

static bool AssertAreEqualStrings(string a, string b, char* name, int id) {
	if (a != b) {
		cout << "unit test " << name << id << " FAILED\n";
	}
	return (a == b);
}
