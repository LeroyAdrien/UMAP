import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
import copy
from sklearn.metrics.pairwise import euclidean_distances
from scipy import optimize
from sklearn.manifold import SpectralEmbedding
from matplotlib import cm
import matplotlib.patches as mpatches

def extraction_CSV(nomfichier):
    
    """Get data of points in n dimensions from a CSV's file,
    return a position's matrix, the label of each point and the number of samples """
    
    expr = pd.read_csv('MNIST.csv', sep=',')
    y_train = expr.values[:,0] # Les Labels (pour les couleurs du plot)
    X_train = expr.values[:,1:] # Les Coordonnées dans l'espace à 784 dimensions
    X_train = np.log(X_train + 1)
    n = X_train.shape[0] # Nombre d'échantillons
    return(X_train,y_train,n)

def calcule_distances_euclidiennes(X_train):
    
    """Calcule des distances euclidiennes de chaque point à partir d'une liste de points dans n dimension,
    renvoit la matrice de distance ainsi que la liste pour chaque point de la distance à son voisin le plus proche"""
    
    dist = np.square(euclidean_distances(X_train,X_train)) #Matrice des distances euclidiennes des échantillons
    rho=[sorted(dist[i])[1] for i in range(len(dist))] #Pour chaque échantillon on prend le voisin le plus proche
    return(dist,rho)
    
def prob_high_dim(sigma, dist_row, dist, rho):
    """
    For each row of Euclidean distance matrix (dist_row) compute
    probability in high dimensions (1D array)
    """
    d = dist[dist_row] - rho[dist_row] 
    d[d < 0] = 0
    return np.exp(- d / sigma)

def K(prob):
    """
    Compute n_neighbor = k (scalar) for each 1D array of high-dimensional probability
    """
    return np.power(2, np.sum(prob))
    
def sigma_binary_search(perp_of_sigma, fixed_perplexity):
    """
    Solve equation perp_of_sigma(sigma) = fixed_perplexity 
    with respect to sigma by the binary search algorithm
    """
    sigma_lower_limit = 0
    sigma_upper_limit = 1000
    for i in range(20):
        approx_sigma = (sigma_lower_limit + sigma_upper_limit) / 2
        if perp_of_sigma(approx_sigma) < fixed_perplexity:
            sigma_lower_limit = approx_sigma
        else:
            sigma_upper_limit = approx_sigma
        if np.abs(fixed_perplexity - perp_of_sigma(approx_sigma)) <= 1e-5:
            break
    return approx_sigma
    
    
def prob_low_dim_UMAP(Y, a, b):
    """
    Compute matrix of probabilities q_ij in low-dimensional space
    """
    inv_distances = np.power(1 + a * np.square(euclidean_distances(Y, Y))**b, -1)
    return(inv_distances)
    
    
def CE(P, Y, a, b):
    """
    Compute Cross-Entropy (CE) from matrix of high-dimensional probabilities 
    and coordinates of low-dimensional embeddings
    """
    Q = prob_low_dim_UMAP(Y, a, b)
    return - P * np.log(Q + 0.01) - (1 - P) * np.log(1 - Q + 0.01)
    
def CE_gradient(P, Y, a, b):
    """
    Compute the gradient of Cross-Entropy (CE)
    """
    y_diff = np.expand_dims(Y, 1) - np.expand_dims(Y, 0)
    inv_dist = np.power(1 + a * np.square(euclidean_distances(Y, Y))**b, -1)
    Q = np.dot(1 - P, np.power(0.001 + np.square(euclidean_distances(Y, Y)), -1))
    np.fill_diagonal(Q, 0)
    Q = Q / np.sum(Q, axis = 1, keepdims = True)
    fact=np.expand_dims(a*P*(1e-8 + np.square(euclidean_distances(Y, Y)))**(b-1) - Q, 2)
    return 2 * b * np.sum(fact * y_diff * np.expand_dims(inv_dist, 2), axis = 1)
    
    
def EnregistrementCSV(listeND,nomfichier,y_train):
        fichier = open(nomfichier,"w")
        for i in range(len(listeND)):
                for j in listeND[i]:
                    fichier.write(str(j))
                    fichier.write(",")
                fichier.write(str(y_train.astype(int)[i]))
                fichier.write("\n")
        fichier.close()

def f(x, min_dist):
    y = []
    for i in range(len(x)):
        if(x[i] <= min_dist):
            y.append(1)
        else:
            y.append(np.exp(- x[i] + min_dist))
    return y
    
def UMAP_func(K_NEIGHBOR, X_train, y_train, n, dist, rho,MIN_DIST):
    X_train,y_train,n=extraction_CSV("MNIST.CSV")
    dist,rho=calcule_distances_euclidiennes(X_train)
    x = np.linspace(0, 3, 300)

    dist_low_dim = lambda x, a, b: 1 / (1 + a*x**(2*b))

    p , _ = optimize.curve_fit(dist_low_dim, x, f(x, MIN_DIST))
    a=p[0]
    b=p[1]
    N_LOW_DIMS = 3
    LEARNING_RATE = 1
    MAX_ITER = 100
    np.random.seed(12345)
    prob_UMAP = np.zeros((n,n))
    sigma_array_UMAP = []
    
    
    for dist_row in range(n):
        func = lambda sigma: K(prob_high_dim(sigma, dist_row, dist, rho))
        binary_search_result = sigma_binary_search(func, K_NEIGHBOR)
        prob_UMAP[dist_row] = prob_high_dim(binary_search_result, dist_row, dist, rho)
        sigma_array_UMAP.append(binary_search_result)
        if (dist_row + 1) % 100 == 0:
            print("Sigma binary search finished {0} of {1} cells".format(dist_row + 1, n))
    print("\nMean sigma = " + str(np.mean(sigma_array_UMAP)))
    
    
    #P = prob_UMAP + np.transpose(prob_UMAP) - np.multiply(prob_UMAP, np.transpose(prob_UMAP))
    P = (prob_UMAP + np.transpose(prob_UMAP)) / 2
    
    
    
    model = SpectralEmbedding(n_components = N_LOW_DIMS, n_neighbors = K_NEIGHBOR)
    y = model.fit_transform(np.log(X_train + 1))
    
    CE_array = []
    print("Running Gradient Descent: \n")
    for i in range(MAX_ITER):

        y = y - LEARNING_RATE * CE_gradient(P, y, a, b)
        
        #EnregistrementCSV(y,nomfichier,y_train)
        
    
        #fig=plt.figure(figsize=(20,15))
        #ax = fig.add_subplot(111, projection='3d')
        #ax.scatter(y[:,0], y[:,1], y[:,2], c = y_train.astype(int), cmap = 'tab10', s = 50) 
        
        #plt.scatter(y[:,0], y[:,1], c = y_train.astype(int), cmap = 'tab10', s = 50) 
        #plt.title("UMAP on MNIST: Programmed from Scratch", 
        #          fontsize = 20)
        #plt.xlabel("UMAP1", fontsize = 20); plt.ylabel("UMAP2", fontsize = 20)
        #plt.savefig('UMAP_Plots_MNIST/UMAP_iter_' + str(i) + '.png')
        #plt.close()
        
        
        
        CE_current = np.sum(CE(P, y, a, b)) / 1e+5
        CE_array.append(CE_current)
        if i % 10 == 0:
            print("Cross-Entropy = " + str(CE_current) + " after " + str(i) + " iterations")
            
    nomfichier="../CSVs/NN_"+str(K_NEIGHBOR)+"_DISTMIN_"+str(MIN_DIST)+".csv"
    
    EnregistrementCSV(y,nomfichier,y_train)
    #plt.figure(figsize=(20,15))
    #plt.plot(CE_array)
    #plt.title("Cross-Entropy", fontsize = 20)
    #plt.xlabel("ITERATION", fontsize = 20); plt.ylabel("CROSS-ENTROPY", fontsize = 20)
    #plt.show()
    
    
    #plt.figure(figsize=(20,15))
    #plt.scatter(y[:,0], y[:,1], c = y_train.astype(int), cmap = 'tab10', s = 50)
    #plt.title("UMAP on MNIST", fontsize = 20)
    #plt.xlabel("UMAP1", fontsize = 20); plt.ylabel("UMAP2", fontsize = 20)


    #cmap = cm.get_cmap('tab10', 10)
    #listePopulations=[]
    #for i in range(10):
    #    listePopulations.append(mpatches.Patch(color=cmap.colors[i], label=str(i)))

    #plt.legend(handles=listePopulations)
    #plt.show()
    
    
def Launch_UMAP(nOfNeighbours,minDistance,path):
    expr = pd.read_csv(path, sep=',')
    y_train = expr.values[:,0] # Les Labels (pour les couleurs du plot)
    X_train = expr.values[:,1:] # Les Coordonnées dans l'espace à 784 dimensions
    X_train = np.log(X_train + 1)
    n = X_train.shape[0] # Nombre d'échantillons
    print("\nThis data set contains " + str(n) + " samples")
    print("\nDimensions of the  data set: ")
    print(X_train.shape)
    dist = np.square(euclidean_distances(X_train,X_train)) #Matrice des distances euclidiennes des échantillons
    rho=[sorted(dist[i])[1] for i in range(len(dist))] #Pour chaque échantillon on prend le voisin le plus proche
    UMAP_func(nOfNeighbours,X_train, y_train, n, dist, rho,minDistance)

for nbOfNeighbours in range (1,3):
    for minDistance in np.logspace(-4,0,20):
        Launch_UMAP(nbOfNeighbours,minDistance,'MNIST.csv')


