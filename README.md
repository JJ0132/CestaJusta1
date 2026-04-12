# CestaJusta

> Your perfect diet, tailored to you.

## About the project

**CestaJusta** is an innovative app designed to break down the financial barrier and challenge the perception that healthy eating is expensive. Given the alarming statistic that 55.8% of the adult population in Spain is overweight or obese, this project was created with the mission of improving public health and generating a positive social impact.

The tool automates the planning of healthy weekly meal plans that strictly adhere to each user’s available budget, while also taking into account their specific nutritional needs, such as allergies or diabetes.

## Social Impact and Rationale
* 55.8% of Spanish adults over the age of 18 (2020 dates) are overweight or obese.
* One of the main barriers identified is the belief that eating healthy is very expensive and difficult to manage.
* Our approach has a significant impact on the lives of many people by tailoring our plans to a wide range of household budgets.

## Objectives

* **Economic access:** Delete the prices barrier between healthy meals and familiar budgets.
* **Public health:** Improve public health by providing affordable, personalized, and healthy meal plans accord to budgets and nutritional needs.
* **Productivity and organization:** Eliminate the worry of doubts of what eat by creating complete meal plans for the week and helping better management for your budget.
* **Specific Diets:** Ensure that the meal plans created are suitable for various allergies or medical conditions, such as diabetes.

## Key Features

* **Custom and Affordable Menus:** Generation of meal plans that align with a high percentage of the budget selected by the user.
* **Dietary Inclusion:** Filters and automatic adjustments for various medical conditions and dietary restrictions.
* **Stress Reduction:** It takes the mental burden off deciding what to eat every day and makes it easier to manage the family finances.
* **Web Scraper:** A custom-built tool for tracking and storing real-time food price history.

## Used technology

* **Programming language:** C# (.NET)
* **Scraping:** Microsoft Playwright
* **Database:** SQL Server (`MercadonaDB`)
* **Data management:** Dapper / Microsoft.Data.SqlClient

### Prerequisites
1. Install [.NET SDK](https://dotnet.microsoft.com/).
2. Have **SQL Server** locally executed as (`localhost\SQLEXPRESS`). The database has to be named `MercadonaDB` and contains the table `Precio_Historico`.
3. Install the Playwright browser binaries by running the package's installation script once it has been compiled.

### Execution
1. Clone the reposirory.
2. Restore the packets (Dapper, Playwright, SqlClient).
3. Compile and execute the proyect.
4. The bot will initialize Chrome and navigate through the caregories of Mercadona and will register all the new products and prices on the database.

## 👥 Team

Developed as a project for Proyectos I subject for computer engineering in Universidad Francisco de Vitoria by:
* **Johan José Mérida Pérez**
* **Alonso Fontecha Pérez**
* **Juan Cisneros Amengual**
* **Hugo Sanz Hernandez**