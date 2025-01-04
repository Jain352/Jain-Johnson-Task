using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Interfaces
interface IEmployee
{
    string Name { get; }
    int Id { get; }
    string Role { get; }
    decimal BasicPay { get; }
    decimal Allowances { get; }
    decimal CalculateSalary();
}

// Base Class
class BaseEmployee : IEmployee
{
    public string Name { get; private set; }
    public int Id { get; private set; }
    public string Role { get; private set; }
    public decimal BasicPay { get; private set; }
    public decimal Allowances { get; private set; }

    public BaseEmployee(string name, int id, string role, decimal basicPay, decimal allowances)
    {
        Name = name;
        Id = id;
        Role = role;
        BasicPay = basicPay;
        Allowances = allowances;
    }

    public virtual decimal CalculateSalary()
    {
        decimal deductions = 0.1m * BasicPay; // Example: 10% deductions
        return BasicPay + Allowances - deductions;
    }
}

// Specialized Classes
class Manager : BaseEmployee
{
    public Manager(string name, int id, decimal basicPay, decimal allowances)
        : base(name, id, "Manager", basicPay, allowances) { }
}

class Developer : BaseEmployee
{
    public Developer(string name, int id, decimal basicPay, decimal allowances)
        : base(name, id, "Developer", basicPay, allowances) { }
}

class Intern : BaseEmployee
{
    public Intern(string name, int id, decimal basicPay, decimal allowances, bool isIntern)
        : base(name, id, "Intern", basicPay, allowances) { }
}

// Main Program
class PayrollSystem
{
    private List<IEmployee> employees = new List<IEmployee>();
    private const string filePath = "employees.txt";

    public void Run()
    {
        while (true)
        {
            Console.WriteLine("\n1. Add New Employee");
            Console.WriteLine("2. Display All Employees");
            Console.WriteLine("3. Calculate and Display Employee Salary");
            Console.WriteLine("4. Display Total Payroll");
            Console.WriteLine("5. Save Employee Data");
            Console.WriteLine("6. Exit");
            Console.Write("Choose an option: ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        AddEmployee();
                        break;
                    case 2:
                        DisplayEmployees();
                        break;
                    case 3:
                        CalculateEmployeeSalary();
                        break;
                    case 4:
                        DisplayTotalPayroll();
                        break;
                    case 5:
                        SaveEmployeesToFile();
                        break;
                    case 6:
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.");
            }
        }
    }

    private void AddEmployee()
    {
        Console.Write("Enter Name: ");
        string name = Console.ReadLine();

        Console.Write("Enter ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id) || employees.Any(e => e.Id == id))
        {
            Console.WriteLine("Invalid or duplicate ID.");
            return;
        }

        Console.Write("Enter Role (Manager/Developer/Intern): ");
        string role = Console.ReadLine();

        Console.Write("Enter Basic Pay: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal basicPay))
        {
            Console.WriteLine("Invalid Basic Pay.");
            return;
        }

        Console.Write("Enter Allowances: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal allowances))
        {
            Console.WriteLine("Invalid Allowances.");
            return;
        }

        IEmployee employee = role switch
        {
            "Manager" => new Manager(name, id, basicPay, allowances),
            "Developer" => new Developer(name, id, basicPay, allowances),
            "Intern" => new Intern(name, id, basicPay, allowances, true),
            _ => null
        };

        if (employee == null)
        {
            Console.WriteLine("Invalid role. Employee not added.");
            return;
        }

        employees.Add(employee);
        Console.WriteLine("Employee added successfully.");
    }

    private void DisplayEmployees()
    {
        if (employees.Count == 0)
        {
            Console.WriteLine("No employees to display.");
            return;
        }

        foreach (var employee in employees)
        {
            Console.WriteLine($"Name: {employee.Name}, ID: {employee.Id}, Role: {employee.Role}, Basic Pay: {employee.BasicPay}, Allowances: {employee.Allowances}");
        }
    }

    private void CalculateEmployeeSalary()
    {
        Console.Write("Enter Employee ID: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            var employee = employees.FirstOrDefault(e => e.Id == id);
            if (employee != null)
            {
                Console.WriteLine($"Salary for {employee.Name} ({employee.Role}): {employee.CalculateSalary()}");
            }
            else
            {
                Console.WriteLine("Employee not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid ID.");
        }
    }

    private void DisplayTotalPayroll()
    {
        if (employees.Count == 0)
        {
            Console.WriteLine("No employees to calculate payroll.");
            return;
        }

        decimal totalPayroll = employees.Sum(e => e.CalculateSalary());
        Console.WriteLine($"Total Payroll: {totalPayroll}");
    }

    private void SaveEmployeesToFile()
    {
        using (StreamWriter writer = new StreamWriter(filePath, false))
        {
            foreach (var employee in employees)
            {
                writer.WriteLine($"{employee.Name},{employee.Id},{employee.Role},{employee.BasicPay},{employee.Allowances}");
            }
        }
        Console.WriteLine("Employee data saved to file.");
    }

    private void LoadEmployeesFromFile()
    {
        employees.Clear();

        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
            Console.WriteLine("No employee data file found. A new file has been created.");
            return;
        }

        string[] lines = File.ReadAllLines(filePath);
        if (lines.Length == 0)
        {
            Console.WriteLine("Employee data file is empty. Please add employees and save data.");
            return;
        }

        foreach (var line in lines)
        {
            try
            {
                var parts = line.Split(',');
                if (parts.Length == 5)
                {
                    string name = parts[0];
                    int id = int.Parse(parts[1]);
                    string role = parts[2];
                    decimal basicPay = decimal.Parse(parts[3]);
                    decimal allowances = decimal.Parse(parts[4]);

                    IEmployee employee = role switch
                    {
                        "Manager" => new Manager(name, id, basicPay, allowances),
                        "Developer" => new Developer(name, id, basicPay, allowances),
                        "Intern" => new Intern(name, id, basicPay, allowances, true),
                        _ => null
                    };

                    if (employee != null)
                    {
                        employees.Add(employee);
                    }
                    else
                    {
                        Console.WriteLine($"Invalid role in file: {line}");
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid line format: {line}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing line: {line}. Details: {ex.Message}");
            }
        }

        if (employees.Count == 0)
        {
            Console.WriteLine("No valid employee records found in the file.");
        }
        else
        {
            Console.WriteLine("Employee data loaded from file.");
        }
    }

    public static void Main(string[] args)
    {
        PayrollSystem payrollSystem = new PayrollSystem();
        payrollSystem.LoadEmployeesFromFile(); // Automatically load data on startup
        payrollSystem.Run();
    }
}
