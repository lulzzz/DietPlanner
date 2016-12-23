result = shapiro.test(data)

pv = result$p.value
w = as.numeric(result$statistic)