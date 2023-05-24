using System;
using System.IO;

public class MainClass
{
    public static void Main()
    {
        Console.WriteLine("Выберите действие:");
        Console.WriteLine("1. Решить уравнение теплопроводности");
        Console.WriteLine("2. CalculateTemperature()");

        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                HeatEquationSolver();
                break;
            case "2":
                CalculateTemperature();
                break;
            default:
                Console.WriteLine("Неправильный выбор.");
                break;
        }

        Console.ReadKey();
    }

    public static void HeatEquationSolver()
    {
        int mf = 500;
        int i, j, N;
        double[] T = new double[mf + 1];
        double[] TT = new double[mf + 1];
        double a, lamda, ro, c, h, tau;
        double Tl, T0, Tr, L, t_end, time;
        string path = "output.txt";

        Console.WriteLine("Введите количество узлов по пространственной координате, N");
        N = int.Parse(Console.ReadLine());

        Console.WriteLine("Введите окончание по времени, t_end");
        t_end = double.Parse(Console.ReadLine());

        Console.WriteLine("Введите толщину пластины, L");
        L = double.Parse(Console.ReadLine());

        Console.WriteLine("Введите коэффициент теплопроводности материала пластины, lamda");
        lamda = double.Parse(Console.ReadLine());

        Console.WriteLine("Введите плотность материала пластины, ro");
        ro = double.Parse(Console.ReadLine());

        Console.WriteLine("Введите теплоемкость материала пластины, c");
        c = double.Parse(Console.ReadLine());

        Console.WriteLine("Введите начальную температуру, T0");
        T0 = double.Parse(Console.ReadLine());

        Console.WriteLine("Введите температуру на границе х=0, Tl");
        Tl = double.Parse(Console.ReadLine());

        Console.WriteLine("Введите температуру на границе х=L, Tr");
        Tr = double.Parse(Console.ReadLine());

        double x, t;

        h = L / (N - 1);
        a = lamda / (ro * c);
        tau = 0.25 * (h * h) / a;

        using (System.IO.StreamWriter file = new System.IO.StreamWriter(path))
        {
            for (i = 0; i <= N; i++)
            {
                T[i] = T0;
                file.Write(T[i] + " ");
            }
            file.Write("\n");

            for (t = 0; t <= t_end; t += tau)
            {
                T[0] = Tl;
                T[N] = Tr;
                for (i = 1; i < N; i++)
                {
                    TT[i] = T[i] + a * tau * ((T[i - 1] - 2 * T[i] + T[i + 1]) / (h * h));
                }

                for (i = 1; i < N; i++)
                {
                    T[i] = TT[i];
                    file.Write(T[i] + " ");
                }
                file.Write("\n");
            }
        }

        Console.WriteLine("Результаты моделирования сохранены в файле " + path);
    }
    public static void CalculateTemperature()
    {
    const int n = 41; // Количество точек разностной сетки
    double[] teta = new double[n]; // Температуры в каждой точке
    double[] a = new double[n]; // Коэффициенты при teta[i-1]
    double[] b = new double[n]; // Коэффициенты при teta[i]
    double[] c = new double[n]; // Коэффициенты при teta[i+1]
    double[] f = new double[n]; // Свободные члены системы уравнений
    double[] alfa = new double[n]; // Прогоночные коэффициенты
    double[] beta = new double[n]; // Прогоночные коэффициенты

    const double Bi = 2.0; // Число Био
    const double Toc = 0.0; // Температура среды на правой границе
    const double Hvne = 3.0; // Коэффициент теплоотдачи в окружающую среду
    const double Qq = 10.0; // Интенсивность источников тепла
    const double teta0 = 1.0; // Начальная температура
    const double r = 1.0; // Число Куранта
    double h = 1.0 / (n - 1); // Величина шага по пространству
    double tau = r * h * h; // Величина шага по времени
    double perwri = 0.02; // Шаг по времени выдачи результатов в файл данных
    int kwri = 1; // Счетчик выдач в файл данных

    double stime = 0.0; // Текущее время процесса теплопередачи

    // Открываем файл для записи результатов
    using (StreamWriter fout = new StreamWriter("temp1.txt"))
    {
        // Задаем начальную температуру в стержне и записываем ее в файл
        for (int i = 0; i < n; i++)
        {
            teta[i] = teta0;
            fout.WriteLine(h * i + " " + teta[i].ToString("F4") + " ");
        }
        fout.WriteLine(h * (n - 1) + " 1");
        fout.WriteLine("0 1");

        stime = 0;

        // Проводим вычисления температуры на новом временном слое
        while (stime <= tau * 3000.0)
        {
            stime += tau;

            // Вычисляем коэффициенты в системе линейных уравнений
            for (int i = 1; i < n - 1; i++)
            {
                a[i] = r;
                b[i] = -2 * r - 1.0 - Hvne * tau;
                c[i] = r;
                f[i] = -teta[i] - Qq * tau * (1.0 - h * i);
            }
            a[0] = 0;
            b[0] = 1;
            c[0] = 0;
            f[0] = 1;
            b[n - 1] = -2 * r - 1.0 - Bi * Hvne * h * tau;
            c[n - 1] = r * (1.0 + Bi * h);
            f[n - 1] = -teta[n - 1] - Bi * Toc * h * tau;

            // Вычисляем начальные прогоночные коэффициенты
            alfa[1] = 0.0;
            beta[1] = 1.0;

            // Вычисляем все прогоночные коэффициенты
            for (int i = 2; i < n; i++)
            {
            double m = a[i] / (b[i] + c[i] * alfa[i - 1]);
            alfa[i] = -m * c[i];
            beta[i] = (f[i] - beta[i - 1] * c[i]) / (b[i] + a[i] * alfa[i - 1]);
            }
            // Вычисляем температуру на правой границе
            teta[n - 1] = (Bi * Toc * h + beta[n - 2]) / (1.0 - alfa[n - 2] + Bi * h);

            // Вычисляем температуру во всех внутренних точках
            for (int i = n - 2; i > 0; i--)
            {
                teta[i] = alfa[i] * teta[i + 1] + beta[i];
            }

            // С периодичностью по времени perwri записываем в файл результаты (поле температуры)
            if (stime > perwri * kwri)
            {
                for (int i = 0; i < n; i++)
                {
                    fout.WriteLine(h * i + " " + teta[i].ToString("F4") + " ");
                }
                fout.WriteLine(h * (n - 1) + " " + 0);
                fout.WriteLine(h * 0 + " " + 0);
                kwri++;
            }
        }
    }
  }
}