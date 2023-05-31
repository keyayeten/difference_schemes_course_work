using System;
using System.IO;

public class MainClass
{
    public static void Main()
    {
        string choice;
        Console.WriteLine("Выберите действие:");
        Console.WriteLine("1. Решить уравнение теплопроводности");
        Console.WriteLine("2. CalculateTemperature()");

        using (StreamReader sr = new StreamReader("mode_preset.txt"))
        {
            choice = sr.ReadLine();
        }
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

        using (StreamReader sr = new StreamReader("data_preset.txt"))
        {
            N = int.Parse(sr.ReadLine());
            t_end = double.Parse(sr.ReadLine());
            L = double.Parse(sr.ReadLine());
            lamda = double.Parse(sr.ReadLine());
            ro = double.Parse(sr.ReadLine());
            c = double.Parse(sr.ReadLine());
            T0 = double.Parse(sr.ReadLine());
            Tl = double.Parse(sr.ReadLine());
            Tr = double.Parse(sr.ReadLine());
        }

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
        Environment.Exit(0);
    }
    // численноe решения задачи о теплопроводности в стержне конечной длины
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

    StreamReader reader = new StreamReader("data_preset.txt");

    string line;
    double Bi, Toc, Hvne, Qq, teta0, r;

    // Считываем значения переменных из файла
    line = reader.ReadLine(); Bi = double.Parse(line);
    line = reader.ReadLine(); Toc = double.Parse(line);
    line = reader.ReadLine(); Hvne = double.Parse(line);
    line = reader.ReadLine(); Qq = double.Parse(line);
    line = reader.ReadLine(); teta0 = double.Parse(line);
    line = reader.ReadLine(); r = double.Parse(line);

    // Закрываем файл
    reader.Close();

    double h = 1.0 / (n - 1); // Величина шага по пространству
    double tau = r * h * h; // Величина шага по времени
    double perwri = 0.01; // Шаг по времени выдачи результатов в файл данных
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
    Environment.Exit(0);
  }
}