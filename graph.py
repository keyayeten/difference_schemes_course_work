import numpy as np
import matplotlib.pyplot as plt
import os


def heat_equation_graph_ver2():
    # Чтение данных из файла output.txt
    data = []
    with open("output.txt", "r") as file:
        for line in file:
            row = [float(x) for x in line.split()]
            if data and len(row) < len(data[0]):
                row += [0]*(len(data[0])-len(row))
            data.append(row)

    # Построение графика
    plt.figure(figsize=(8, 6))
    plt.imshow(data, aspect='auto')
    plt.colorbar()
    plt.xlabel('Coordinate')
    plt.ylabel('Time')
    plt.title('Temperature distribution')
    plt.show()


# как в примере
def heat_equation_graph(data):
    # Определяем количество узлов по пространственной координате N
    for results in data:
        N = results.shape[1]

        # Устанавливаем значения параметров
        L = 1.0
        h = L / (N - 1)

        # Выбираем значение времени t=20
        t_index = int(20 / 0.25)
        T = results[t_index, :]

        # Определяем координаты x
        x = np.linspace(0, L, N)
        plt.plot(x, T)

    plt.title('Распределение температуры по толщине пластины\
            в момент времени t = 20')
    plt.xlabel('x')
    plt.ylabel('Температура, °C')
    plt.show()


def line_method():
    with open('temp1.txt', 'r') as file:
        data = list(map(lambda x: tuple(map(float, x.split())),
                        file.read().split('\n')))[0:-1:]
        # Создание графика
    plt.plot(list(map(lambda x: x[0], data)),
             list(map(lambda x: x[1], data)))

    # Добавление заголовка и подписей к осям
    plt.title('стационарное распределение температуры')
    plt.xlabel('ξ')
    plt.ylabel('θ')

    # Отображение графика
    plt.show()

# Результаты расчетов по приведенной программе при L = 0,1 м,
# λ = 46 Вт/(м⋅ºC), ρ = 7800 кг/м, с = 460 Дж/(кг⋅ºC),
# Т0=20 °С, Т1 = 300 °С,
# Т2 = 100 °С через 60 секунд процесса нагрева.


def show_heat_equation():
    # N t_end L lambda r0 c T0  Tl  Tr
    data_preset = [f'50 60 0.1 46 7800 460 {i} 300 300'
                   for i in range(20, 301, 40)]

    info_for_graphics = []
    with open('mode_preset.txt', 'w') as file:
        file.write('1')
    for data in data_preset:
        with open('data_preset.txt', 'w') as file:
            file.write('\n'.join(data.split()))
        command = "dotnet run"
        os.system(command)
        info_for_graphics.append(np.loadtxt('output.txt', usecols=range(49)))

    heat_equation_graph(info_for_graphics)


# show_heat_equation()

# heat_equation_graph_ver2()


# Стационарное распределение температуры

    # Bi  Число Био
    # Toc   Температура среды на правой границе
    # Hvne   Коэффициент теплоотдачи в окружающую среду
    # Qq  Интенсивность источников тепла
    # teta0  Начальная температура
    # r  Число Куранта
def show_st():
    with open('mode_preset.txt', 'w') as file:
        file.write('2')
                  #Bi  Toc Hvne Qq teta0  r
    data_preset = '1.0 0.0 0.0 0.0 1.0 1.0'
    with open('data_preset.txt', 'w') as file:
        file.write('\n'.join(data_preset.split()))
    command = "dotnet run"
    os.system(command)
    line_method()


show_st()
