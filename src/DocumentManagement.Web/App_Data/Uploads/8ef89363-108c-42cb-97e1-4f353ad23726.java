
import java.util.Scanner;
import java.util.TreeSet;
import java.util.regex.*;

public class Intersection {

	public static void main(String[] args) {

		Pattern inputPattern = Pattern.compile("^(\\[.*?\\])(\\+|\\*|\\-)(\\[.*?\\])$");

		Scanner in = new Scanner(System.in);
		
		String userInput = "";
		String A = "";
		String B = "";
		String operator = "";

		System.out.println("Please input sets with operator(+,*,-):[example: [1,2,3]+[4,5,6]");
		while (true) {
			Matcher matcher = inputPattern.matcher(in.next());
			if (matcher.matches()) {
				if (matcher.groupCount() == 3) {
					userInput = matcher.group(0);
					A = matcher.group(1);
					operator = matcher.group(2).trim();
					B = matcher.group(3);

					System.out.println(userInput);
					
					break;
				}
			}
			else {
				System.out.println("The input is not in correct format, Pleas try again!");
			}
		}
		
		in.close();
		
		TreeSet<Integer> setA = createTreeSet(A);
		TreeSet<Integer> setB = createTreeSet(B);
		
		if (operator.equals("+")) {
			setA.addAll(setB);
			System.out.println("Treeset union: " + setA);
		}
		else if (operator.equals("*")) {
			setA.retainAll(setB);
			System.out.println("Treeset intersect: " + setA);
		}
		else if (operator.equals("-")) {
			setA.removeAll(setB);
			System.out.println("Treeset difference: " + setA);
		} 
		else {
			System.out.println("no valid operators!");
		}
	}
	
	private static TreeSet<Integer> createTreeSet(String val) {
		TreeSet<Integer> result = new TreeSet<Integer>();

		if (val.length() > 0) {
			val = val.replace('[', ' ').replace(']', ' ').trim();
			
			String[] vals  = val.split(",");
			for(int i =0;i < vals.length; i++) {
				result.add(new Integer(vals[i]));
			}
		}
		
		return result;
	}


}
