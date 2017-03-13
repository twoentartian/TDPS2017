image = imread('DEBUG1.jpg');
image_0_1=imresize(image,0.1);
imageGray = rgb2gray(image_0_1);

for x = 1:416
    for y = 1:312
        if imageGray(x,y) < 120
            
        else
            imageGray(x,y) = 255;
        end
    end
end

figure();
imshow(imageGray);
figure();
mesh(imageGray);