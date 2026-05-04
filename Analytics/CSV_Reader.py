import pandas as pd
import matplotlib.pyplot as plt
import os


def read_csv_and_plot():
    #  Load the data from csv file
    export_dir = os.path.join(os.getcwd(), 'Exports')
    df = pd.read_csv(os.path.join(export_dir, 'stats.csv'))

    #  Create a professional multi-plot figure
    fig, ax1 = plt.subplots()

    ax1.set_xlabel('Time (seconds)')
    ax1.set_ylabel('Population', color='tab:blue')
    ax1.plot(df['Timestamp'], df['CritterCount'], label='Critters', color='tab:blue')
    ax1.plot(df['Timestamp'], df['PredatorCount'], label='Predators', color='tab:red')

    ax2 = ax1.twinx()
    ax2.set_ylabel('Avg Speed', color='tab:green')
    ax2.plot(df['Timestamp'], df['AvgCritterSpeed'], label='Critter Speed', color='tab:green', linestyle='--')
    ax2.plot(df['Timestamp'], df['AvgPredatorSpeed'], label='Predator Speed', color='tab:orange', linestyle='--')

    plt.title('Ecosystem Population vs. Evolutionary Traits')
    fig.legend(loc='upper left', bbox_to_anchor=(0.1, 0.9))
    plt.grid()
    plt.savefig(os.path.join('plots', 'stats_plot.png'))

if __name__ == "__main__":
    # Ensure the directory for plots exists
    os.makedirs('plots', exist_ok=True)
    read_csv_and_plot()