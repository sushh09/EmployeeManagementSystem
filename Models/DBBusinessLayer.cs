using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace EmployeeManagementSystem.Models
{
    public class DBBusinessLayer
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["BusinessLayer_DBConnection"].ConnectionString;
        public bool AddEmployee(Employee employee)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["BusinessLayer_DBConnection"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("spAddEmployee", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramName = new SqlParameter();
                    paramName.ParameterName = "@E_Name";
                    paramName.Value = employee.EmployeeName;
                    cmd.Parameters.Add(paramName);

                    SqlParameter paramEmployeeCode = new SqlParameter();
                    paramEmployeeCode.ParameterName = "@E_EmployeeCode";
                    paramEmployeeCode.Value = employee.EmployeeCode;
                    cmd.Parameters.Add(paramEmployeeCode);

                    SqlParameter paramEmailId = new SqlParameter();
                    paramEmailId.ParameterName = "@E_EmailId";
                    paramEmailId.Value = employee.EmailId;
                    cmd.Parameters.Add(paramEmailId);

                    SqlParameter paramPhoneNo = new SqlParameter();
                    paramPhoneNo.ParameterName = "@E_PhoneNo";
                    paramPhoneNo.Value = employee.PhoneNumber;
                    cmd.Parameters.Add(paramPhoneNo);


                    SqlParameter paramGender = new SqlParameter();
                    paramGender.ParameterName = "@E_Gender";
                    paramGender.Value = employee.Gender;
                    cmd.Parameters.Add(paramGender);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public Employee GetEmployeeByCode(string code)
        {
            Employee employee = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Employees WHERE EmployeeCode = @EmployeeCode";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EmployeeCode", code);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    employee = new Employee
                    {
                        EmployeeCode = reader["EmployeeCode"].ToString(),
                        EmployeeName = reader["EmployeeName"].ToString(),
                        EmailId = reader["EmailId"].ToString(),
                        PhoneNumber = reader["PhoneNumber"].ToString(),
                        Gender = reader["Gender"].ToString()
                    };
                }

                con.Close();
            }

            return employee;
        }
        public bool UpdateEmployee(Employee employee)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"UPDATE Employees
                         SET Name = @EmployeeName, EmailId = @EmailId, PhoneNo = @PhoneNumber, Gender = @Gender 
                         WHERE EmployeeCode = @EmployeeCode";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EmployeeName", employee.EmployeeName);
                cmd.Parameters.AddWithValue("@EmailId", employee.EmailId);
                cmd.Parameters.AddWithValue("@PhoneNumber", employee.PhoneNumber);
                cmd.Parameters.AddWithValue("@Gender", employee.Gender);
                cmd.Parameters.AddWithValue("@EmployeeCode", employee.EmployeeCode);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();

                return rowsAffected > 0;
            }
        }
        public bool DeleteEmployee(string code)
{
    using (SqlConnection con = new SqlConnection(connectionString))
    {
        string query = "DELETE FROM Employees WHERE EmployeeCode = @EmployeeCode";
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@EmployeeCode", code);

        con.Open();
        int rowsAffected = cmd.ExecuteNonQuery();
        con.Close();

        return rowsAffected > 0;
    }
}
        public List<Employee> ViewEmployeesDetails()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["BusinessLayer_DBConnection"].ConnectionString;
            List<Employee> employees = new List<Employee>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllEmployees", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Employee employee = new Employee();
                    employee.EmployeeID = Convert.ToInt32(rdr["EmployeeId"]);
                    employee.EmployeeName = rdr["EmployeeName"].ToString();
                    employee.EmployeeCode = rdr["EmployeeCode"].ToString();
                    employee.EmailId = rdr["EmailId"].ToString();
                    employee.PhoneNumber = rdr["PhoneNumber"].ToString();
                    employee.Gender = rdr["Gender"].ToString();

                    employees.Add(employee);
                }
                rdr.Close();
            }
            return employees;
        }

        #region
        // Add a new method to search employees by code
        //public List<Employee> SearchEmployeesByCode(string employeeCode)
        //{
        //    string connectionString = ConfigurationManager.ConnectionStrings["BusinessLayer_DBConnection"].ConnectionString;
        //    List<Employee> employees = new List<Employee>();
        //    using (SqlConnection con = new SqlConnection(connectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spSearchEmployeesByCode", con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);

        //        con.Open();
        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        while (rdr.Read())
        //        {
        //            Employee employee = new Employee();
        //            employee.EmployeeID = Convert.ToInt32(rdr["EmployeeID"]);
        //            employee.Name = rdr["Name"].ToString();
        //            employee.EmployeeCode = rdr["EmployeeCode"].ToString();
        //            employee.EmailId = rdr["EmailId"].ToString();
        //            employee.PhoneNo = rdr["PhoneNo"].ToString();
        //            employee.Gender = rdr["Gender"].ToString();

        //            employees.Add(employee);
        //        }
        //    }
        //    return employees;
        //}
        #endregion
      
        public List<Employee> SearchEmployeesByAllFields(Employee employee)
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["BusinessLayer_DBConnection"].ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spSearchEmployees", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Name", employee.EmployeeName);
                    cmd.Parameters.AddWithValue("@EmployeeCode", employee.EmployeeCode);
                    cmd.Parameters.AddWithValue("@EmailId", employee.EmailId);
                    cmd.Parameters.AddWithValue("@PhoneNo", employee.PhoneNumber);
                    cmd.Parameters.AddWithValue("@Gender", employee.Gender);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Employee emp = new Employee();
                        emp.EmployeeID = Convert.ToInt32(rdr["EmployeeID"]);
                        emp.EmployeeName = rdr["Name"].ToString();
                        emp.EmployeeCode = rdr["EmployeeCode"].ToString();
                        emp.EmailId = rdr["EmailId"].ToString();
                        emp.PhoneNumber = rdr["PhoneNo"].ToString();
                        emp.Gender = rdr["Gender"].ToString();
                        employees.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return employees;

        }
    }
}
