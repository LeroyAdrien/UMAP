# UMAP Algorithm: Code for a university presentation


## The presentation:

[![Alt text](https://img.youtube.com/vi/VPq4Ktf2zJ4/0.jpg)](https://www.youtube.com/watch?v=VPq4Ktf2zJ4)

## Methodology:

### Launch UMAP in Python:

```python
#You need to install the library umap-learn

import numpy as np
from sklearn.datasets import load_digits
import matplotlib.pyplot as plt
import umap.umap_ as umap

digits = load_digits()
data = digits.data

reducer = umap.UMAP(min_dist=0.1, n_components=2, n_neighbors=15, verbose=True)
reducer.fit(digits.data)

embedding = reducer.transform(digits.data)
# Verify that the result of calling transform is
# identical to accessing the embedding_ attribute
assert(np.all(embedding == reducer.embedding_))
embedding.shape

plt.scatter(embedding[:, 0], embedding[:, 1], c=digits.target, cmap='Spectral', s=5)
plt.gca().set_aspect('equal', 'datalim')
plt.colorbar(boundaries=np.arange(11)-0.5).set_ticks(np.arange(10))
plt.title('UMAP projection of the Digits dataset', fontsize=24);
```


### Launch UMAP in R:
```r
library(umap)
iris.umap = umap(MNIST.data)
plot.iris(MNIST.umap, MNIST.labels)
```

## Structure of the repository:

* CSVs : UMAP results computed using the python script on the MNIST Dataset
* Executabes : Ready to launch UMAP animations, simply download the zip corresponding to your operating system to test the program
* Python : Python scripts to understand/Launch UMAP and to generate results 
	* Notebooks : Python scripts in jupyter Notebook for simplicty
	* UMAP.py : UMAP python script coded from scratch, used to generate the results
* UMAP_Animation : Unity code to animate UMAP results on the MNIST dataset

## How to test the animations quickly:

Simply download the zip files in the executables folder corresponding to your operating system and launch them.
**MAC OSX** : Don't forget to add the executable as trustworthy to be able to launch it. To do so, **hold** `ctrl` while clicking on the executable and click `open`.
 

https://youtu.be/VPq4Ktf2zJ4
