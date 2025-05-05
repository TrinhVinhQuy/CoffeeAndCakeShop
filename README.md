 CakeCoffeeShop

CakeCoffeeShop is a web-based application built with ASP.NET Core 7 that allows users to browse, search, and order cakes and coffee online. This project demonstrates the use of a 3-layer architecture with an MVC design pattern and modern front-end technologies (HTML, CSS, JavaScript).

## Features

- Display products (cakes and coffee)
- Product filtering and pagination
- Product details view
- Add to cart functionality
- Place orders
- Admin dashboard (CRUD for products, view orders)
- Responsive user interface

## Technologies Used

### Backend
- **ASP.NET Core 7 (MVC)**
- **Entity Framework Core**
- **SQL Server**

### Frontend
- **HTML5**
- **CSS3**
- **Vanilla JavaScript**

## Project Structure

```
CakeCoffeeShop/
├── Controllers/
├── Models/
├── Views/
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── images/
├── Data/
├── Migrations/
├── appsettings.json
└── Program.cs
```

## Setup Instructions

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/CakeCoffeeShop.git
   ```

2. **Navigate to the project directory**
   ```bash
   cd CakeCoffeeShop
   ```

3. **Update the database connection string in `appsettings.json`**

4. **Apply migrations and update the database**
   ```bash
   dotnet ef database update
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Access the app**
   Open your browser and navigate to `https://localhost:5001`

## Screenshots

*(Add screenshots of the homepage, product listing, cart, and admin dashboard)*

## License

This project is licensed under the MIT License.

## Author
Name – [Your Email or GitHub]
